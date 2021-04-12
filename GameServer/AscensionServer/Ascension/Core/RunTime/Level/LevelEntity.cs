using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using NHibernate.Linq.Clauses;
using System.ServiceModel.Configuration;
using AscensionProtocol;

namespace AscensionServer
{
    /// <summary>
    /// 场景实体对象
    /// </summary>
    public class LevelEntity : Entity, IReference, IRefreshable
    {
        /// <summary>
        /// 区域码
        /// 区分历练与秘境的操作码；
        /// </summary>
        public ushort OpCode { get; private set; }
        /// <summary>
        /// None表示当前失活状态；
        /// 用于区分场景属于历练还是秘境类型；
        /// </summary>
        public LevelTypeEnum LevelType { get; private set; }
        /// <summary>
        /// 此Id为单增；
        /// 此Id是用于区分例如秘境等同步人数不多，但是数量众多的散列场景对象；
        /// 如玩家A组队进入秘境场景，则为其分配一个场景对象。玩家B进入秘境，同样分配一个场景；
        /// </summary>
        public long LevelId { get { return Id; } set { Id = value; } }
        /// <summary>
        /// 当前场景对象是否存活；
        /// </summary>
        public bool Available { get; private set; }
        Dictionary<int, RoleEntity> roleDict;
        Action<OperationData> roleSendMsgHandler;
        event Action<OperationData> RoleSendMsgHandler
        {
            add { roleSendMsgHandler += value; }
            remove { roleSendMsgHandler -= value; }
        }
        /// <summary>
        /// 帧对应角色ID的输入指令;
        /// 《帧《RoleID,CMD》》
        /// </summary>
        Dictionary<long, Dictionary<int, CmdInput>> roleInputCmdDict;

        S2CInput InputSet = new S2CInput();

        long currentTick;
        Dictionary<int, SessionRoleIdPair> roleSessionDict;

        SessionRoleIds sessionRoleIds = new SessionRoleIds();

        OperationData refreshOpData = new OperationData();

        ObjectQueue<OperationData> opDataQueue;

        ObjectQueue<SessionRoleIdPair> srPairQueue;

        List<SessionRoleIdPair> existRoles;

        public int PlayerCount { get { return roleDict.Count; } }
        public bool Empty { get { return roleDict.Count==0; } }
        public LevelEntity()
        {
            existRoles = new List<SessionRoleIdPair>();
            roleInputCmdDict = new Dictionary<long, Dictionary<int, CmdInput>>();
            roleDict = new Dictionary<int, RoleEntity>();
            opDataQueue = new ObjectQueue<OperationData>();
            srPairQueue = new ObjectQueue<SessionRoleIdPair>(false);
            roleSessionDict = new Dictionary<int, SessionRoleIdPair>();
        }
        public void OnInit(int sceneId)
        {
            this.LevelId = sceneId;
            this.Available = true;
            this.InputSet.ContainerId = sceneId;
        }
        public bool HasRole(int roleId)
        {
            return roleDict.ContainsKey(roleId);
        }
        public bool EnterLevel(int roleId, RoleEntity role)
        {
            var result = roleDict.TryAdd(roleId, role);
            if (result)
            {
                //进入场景逻辑：
                //1、新玩家进入，发送当前场景中已经存在的其他玩家数据，并将自己添加到场景玩家容器中；
                //2、广播新进入玩家的数据到当前场景中的其他玩家；
                //3、广播完成，将玩家对象广播的接口进行委托监听；
                Utility.Debug.LogWarning($"RoleId:{roleId};SessionId:{role.SessionId}进入Level：{LevelId}");
                var opData = opDataQueue.Dequeue();
                OnEnterLevelS2C(role);
                roleSessionDict.TryGetValue(role.RoleId, out var sessionRoleIdPair);
                opData.DataContract = sessionRoleIdPair;
                roleSendMsgHandler?.Invoke(opData);
                RoleSendMsgHandler += role.SendMessage;
                role.TryAdd(typeof(LevelEntity), this);
                opDataQueue.Enqueue(opData);
            }
            return result;
        }
        public bool PeekRole(int roleId, out RoleEntity role)
        {
            return roleDict.TryGetValue(roleId, out role);
        }
        public bool ExitLevel(int roleId, out RoleEntity role)
        {
            var result = roleDict.Remove(roleId, out role);
            if (result)
            {
                Utility.Debug.LogWarning($"RoleId:{role.RoleId};SessionId:{role.SessionId}离开Level：{LevelId};PlayerCount:{PlayerCount}");
                RoleSendMsgHandler -= role.SendMessage;
                OnExitLevelS2C(roleId);
                role.TryRemove(typeof(LevelEntity));
            }
            return result;
        }
        /// <summary>
        ///接收到消息后直接存储，不考虑顺序 
        /// </summary>
        public void OnCommandC2S(IDataContract data)
        {
            if (data == null)
                return;
            var input = data as CmdInput;
            var result = roleInputCmdDict.TryGetValue(currentTick, out var roleCmdDict);
            if (result)
            {
                roleCmdDict.TryAdd(input.RoleId, input);
            }
            else
            {
                roleInputCmdDict.TryAdd(currentTick, new Dictionary<int, CmdInput>());
                roleInputCmdDict[currentTick].TryAdd(input.RoleId, input);
            }
        }
        /// <summary>
        /// 发送消息到当前场景所有玩家；
        /// Send message to all role server to client
        /// </summary>
        public void SndMsg2AllS2C(OperationData data)
        {
            roleSendMsgHandler?.Invoke(data);
        }
        public void OnRefresh()
        {
            if (!Available)
                return;
            var result = roleInputCmdDict.TryGetValue(currentTick, out var roleCmds);
            InputSet.InputDict = roleCmds;
            InputSet.Tick = currentTick;
            refreshOpData.DataContract = InputSet;
            roleSendMsgHandler?.Invoke(refreshOpData);
            if (result)
            {
                //若当前帧发送成功，则移除上一个逻辑帧数据；服务器当前不存储数据，仅负责转发；
                //Utility.Debug.LogInfo($"LevelId:{LevelId}找到帧，发送,PlayerCout:{PlayerCount},Tick:{currentTick}");
            }
            else
            {
                //Utility.Debug.LogInfo($"LevelId:{LevelId}空帧,PlayerCout:{PlayerCount},Tick:{currentTick}");
            }
            roleInputCmdDict.Remove(currentTick - 1, out _);
            currentTick++;
        }
        public void Clear()
        {
            Utility.Debug.LogWarning($"Level:{LevelId}无玩家，Clear");
            roleDict.Clear();
            this.LevelId = 0;
            this.Available = false;
            roleInputCmdDict.Clear();
            roleSendMsgHandler = null;
            currentTick = 0;
            InputSet.Clear();
            opDataQueue.Clear();
        }
        public static LevelEntity Create(LevelTypeEnum levelType, int sceneId, params RoleEntity[] peerEntities)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            switch (levelType)
            {
                case LevelTypeEnum.Adventure:
                    se.OpCode = (byte)OperationCode.AdventureArea;
                    break;
                case LevelTypeEnum.SecretArea:
                    se.OpCode = (byte)OperationCode.SecretArea;
                    break;
            }
            se.LevelType = levelType;
            se.OnInit(sceneId);
            int length = peerEntities.Length;
            for (int i = 0; i < length; i++)
            {
                se.EnterLevel(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(LevelTypeEnum levelType, int sceneId, List<RoleEntity> peerEntities)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            switch (levelType)
            {
                case LevelTypeEnum.Adventure:
                    se.OpCode = (byte)OperationCode.AdventureArea;
                    break;
                case LevelTypeEnum.SecretArea:
                    se.OpCode = (byte)OperationCode.SecretArea;
                    break;
            }
            se.LevelType = levelType;
            se.OnInit(sceneId);
            int length = peerEntities.Count;
            for (int i = 0; i < length; i++)
            {
                se.EnterLevel(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(LevelTypeEnum levelType, int sceneId)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            switch (levelType)
            {
                case LevelTypeEnum.Adventure:
                    se.OpCode = (byte)OperationCode.AdventureArea;
                    break;
                case LevelTypeEnum.SecretArea:
                    se.OpCode = (byte)OperationCode.SecretArea;
                    break;
            }
            se.LevelType = levelType;
            se.OnInit(sceneId);
            return se;
        }
        /// <summary>
        /// 将已在场景中的玩家数据发送给进来的角色；
        /// </summary>
        void OnEnterLevelS2C(RoleEntity role)
        {
            var opData = opDataQueue.Dequeue();
            opData.DataContract = sessionRoleIds;
            var sessionRole = srPairQueue.Dequeue();
            sessionRole.RoleId = role.RoleId;
            sessionRole.SessionId = role.SessionId;
            roleSessionDict.TryAdd(role.RoleId, sessionRole);
            existRoles.Clear();
            existRoles.AddRange(roleSessionDict.Values);
            sessionRoleIds.SessionRoleIdList = existRoles;
            role.SendMessage(opData);
        }
        /// <summary>
        /// 将离开的玩家数据广播给已经在level中的其他玩家；
        /// </summary>
        void OnExitLevelS2C(int roleId)
        {
            if (roleSessionDict.Remove(roleId, out var sessionRolePair))
            {
                refreshOpData.DataContract = sessionRolePair;
                var opData = opDataQueue.Dequeue();
                opData.OperationCode = OpCode;
                roleSendMsgHandler?.Invoke(opData);
                opDataQueue.Enqueue(opData);
                srPairQueue.Enqueue(sessionRolePair);
            }
        }
    }
}


