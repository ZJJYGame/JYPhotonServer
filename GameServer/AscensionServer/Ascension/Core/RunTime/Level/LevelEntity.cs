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
        public LevelTypeEnum LevelType { get; private set; }
        public int LevelId { get { return (int)Id; } set { Id = value; } }
        public bool Available { get; private set; }
        ConcurrentDictionary<int, RoleEntity> roleDict;
        Action<OperationData> roleSendMsgHandler;
        event Action<OperationData> RoleSendMsgHandler
        {
            add { roleSendMsgHandler += value; }
            remove{roleSendMsgHandler -= value;}
        }
        Dictionary<long, Dictionary<int, C2SInput>> roleInputCmdDict
        = new Dictionary<long, Dictionary<int, C2SInput>>();
        S2CInput InputSet = new S2CInput();
        long currentTick;
        ConcurrentDictionary<int, C2SPlayer> c2sPlayerDict = new ConcurrentDictionary<int, C2SPlayer>();
        ConcurrentQueue<C2SPlayer> c2sPalyerQueue = new ConcurrentQueue<C2SPlayer>();
        S2CPlayer s2cPlayer = new S2CPlayer();

        OperationData operationData = new OperationData();
        ObjectQueue<OperationData> opDataQueue;

        public int PlayerCount { get { return roleDict.Count; } }
        public bool Empty { get { return roleDict.IsEmpty; } }
        public LevelEntity()
        {
            roleDict = new ConcurrentDictionary<int, RoleEntity>();
            operationData.OperationCode =(byte)OperationCode.AdventureArea;
            opDataQueue = new ObjectQueue<OperationData>();
        }
        public void OnInit(int sceneId)
        {
            this.LevelId = sceneId;
            this.Available = true;
            this.InputSet.ContainerId = sceneId;
        }
        public bool ContainsKey(int roleId)
        {
            return roleDict.ContainsKey(roleId);
        }
        public bool TryAdd(int roleId, RoleEntity role)
        {
            var result = roleDict.TryAdd(roleId, role);
            if (result)
            {
                //进入场景逻辑：
                //1、新玩家进入，发送当前场景中已经存在的其他玩家数据，并将自己添加到场景玩家容器中；
                //2、广播新进入玩家的数据到当前场景中的其他玩家；
                //3、广播完成，将玩家对象广播的接口进行委托监听；
                Utility.Debug.LogWarning($"RoleId:{roleId};SessionId:{role.SessionId}进入Level：{LevelId}");
                var opData = operationData.Clone();
                OnEnterLevelS2C(role);
                c2sPlayerDict.TryGetValue(role.RoleId, out var c2sPalyer);
                opData.DataContract = c2sPalyer;
                roleSendMsgHandler?.Invoke(opData);
                RoleSendMsgHandler += role.SendMessage;
                role.TryAdd(typeof(LevelEntity), this);
            }
            return result;
        }
        public bool TryAdd(RoleEntity role)
        {
            var result = roleDict.TryAdd(role.RoleId, role);
            if (result)
            {
                Utility.Debug.LogWarning($"RoleId:{role.RoleId};SessionId:{role.SessionId}进入Level：{LevelId}");
                var opData = operationData.Clone();
                OnEnterLevelS2C(role);
                c2sPlayerDict.TryGetValue(role.RoleId, out var c2sPalyer);
                opData.DataContract = c2sPalyer;
                roleSendMsgHandler?.Invoke(opData);
                RoleSendMsgHandler += role.SendMessage;
                role.TryAdd(typeof(LevelEntity), this);
            }
            return result;
        }
        public bool TryGetValue(int roleId, out RoleEntity role)
        {
            return roleDict.TryGetValue(roleId, out role);
        }
        public bool TryRemove(int roleId)
        {
            var result = roleDict.TryRemove(roleId, out var role);
            if (result)
            {
                Utility.Debug.LogWarning($"RoleId:{roleId};SessionId:{role.SessionId}离开Level：{LevelId};PlayerCount:{PlayerCount}");
                RoleSendMsgHandler -= role.SendMessage;
                OnExitLevelS2C(roleId);
                role.TryRemove(typeof(LevelEntity));
            }
            return result;
        }
        public bool TryRemove(int roleId, out RoleEntity role)
        {
            var result = roleDict.TryRemove(roleId, out role);
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
            var input = data as C2SInput;
            var result = roleInputCmdDict.TryGetValue(currentTick, out var roleCmdDict);
            if (result)
            {
                roleCmdDict.TryAdd(input.PlayerId, input);
            }
            else
            {
                roleInputCmdDict.TryAdd(currentTick, new Dictionary<int, C2SInput>());
                roleInputCmdDict[currentTick].TryAdd(input.PlayerId, input);
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
            InputSet.Tick = (int)currentTick;
            operationData.DataContract = InputSet;
            roleSendMsgHandler?.Invoke(operationData);
            if (result)
            {
                //若当前帧发送成功，则移除上一个逻辑帧数据；服务器当前不存储数据，仅负责转发；
                //Utility.Debug.LogInfo($"LevelId:{LevelId}找到帧，发送,PlayerCout:{PlayerCount},Tick:{currentTick}");
            }
            else
            {
                //Utility.Debug.LogInfo($"LevelId:{LevelId}空帧,PlayerCout:{PlayerCount},Tick:{currentTick}");
            }
            roleInputCmdDict.Remove(currentTick - 1,out _);
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
        }
        public static LevelEntity Create(LevelTypeEnum levelType, int sceneId, params RoleEntity[] peerEntities)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            se.LevelType = levelType;
            se.OnInit(sceneId);
            int length = peerEntities.Length;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(LevelTypeEnum levelType, int sceneId, List<RoleEntity> peerEntities)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            se.LevelType = levelType;
            se.OnInit(sceneId);
            int length = peerEntities.Count;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(LevelTypeEnum levelType,int sceneId)
        {
            LevelEntity se = CosmosEntry.ReferencePoolManager.Spawn<LevelEntity>();
            se.LevelType = levelType;
            se.OnInit(sceneId);
            return se;
        }
        /// <summary>
        /// 将已在场景中的玩家数据发送给进来的角色；
        /// </summary>
        void OnEnterLevelS2C(RoleEntity role)
        {
            //广播的数据为S2CPlayers；
            operationData.DataContract = s2cPlayer;
            if (c2sPalyerQueue.TryDequeue(out var c2sPlayer))
            {
                c2sPlayer.Dispose();
                c2sPlayer.PlayerId = role.RoleId;
                c2sPlayer.SessionId = role.SessionId;
                c2sPlayerDict.TryAdd(role.RoleId, c2sPlayer);
            }
            else
            {
                c2sPlayerDict.TryAdd(role.RoleId, new C2SPlayer(role.SessionId, role.RoleId));
            }
            var c2sPlayers = new List<C2SPlayer>();
            c2sPlayers.AddRange(c2sPlayerDict.Values);
            s2cPlayer.PlayerList = c2sPlayers;
            role.SendMessage(operationData);
        }
        /// <summary>
        /// 将离开的玩家数据广播给已经在level中的其他玩家；
        /// </summary>
        void OnExitLevelS2C(int roleId)
        {
            //广播的数据为C2SPlayer
            if (c2sPlayerDict.TryRemove(roleId, out var c2sPlayer))
            {
                operationData.DataContract = c2sPlayer;
                roleSendMsgHandler?.Invoke(operationData);
                c2sPalyerQueue.Enqueue(c2sPlayer);
            }
        }
    }
}


