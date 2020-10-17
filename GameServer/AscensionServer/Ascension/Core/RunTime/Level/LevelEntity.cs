using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;
namespace AscensionServer
{
    /// <summary>
    /// 场景实体对象
    /// </summary>
    public class LevelEntity : Entity, IReference, IRefreshable
    {
        public int LevelId { get { return (int)Id; }set { Id = value; } }
        public bool Available { get; private set; }
        ConcurrentDictionary<int, RoleEntity> roleDict;
        Action<OperationData> roleSendMsgHandler;
        event Action<OperationData> RoleSendMsgHandler
        {
            add { roleSendMsgHandler += value; }
            remove
            {
                try { roleSendMsgHandler -= value; }
#if SERVER
                catch (Exception e) { Utility.Debug.LogError(e); }
#else
                catch (Exception e) { Utility.DebugError(e); }
#endif
            }
        }
#if SERVER
        Dictionary<long, Dictionary<int, C2SInput>> roleInputCmdDict
        = new Dictionary<long, Dictionary<int, C2SInput>>();
        S2CInput InputSet = new S2CInput();
        long currentTick;
#else
        Dictionary<int, C2SInput> roleInputCmdDict=new Dictionary<int, C2SInput>();
#endif
        OperationData opData = new OperationData();
        public int PlayerCount { get { return roleDict.Count; } }
        public bool Empty { get { return roleDict.IsEmpty; } }
        public LevelEntity()
        {
            roleDict = new ConcurrentDictionary<int, RoleEntity>();
            opData.OperationCode = ProtocolDefine.OPERATION_PLYAER_INPUT;
        }
        public void OnInit(int sceneId)
        {
            this.LevelId = sceneId;
            this.Available = true;
#if SERVER
            this.InputSet.EntityContainerId = sceneId;
#endif
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
                Utility.Debug.LogWarning($"RoleId:{roleId};SessionId:{role.SessionId}进入Level：{LevelId}");
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
                Utility.Debug.LogWarning($"RoleId:{roleId};SessionId:{role.SessionId}离开Level：{LevelId}");
                RoleSendMsgHandler -= role.SendMessage;
                role.TryRemove(typeof(LevelEntity));
            }
            return result;
        }
        public bool TryRemove(int roleId, out RoleEntity role)
        {
            var result = roleDict.TryRemove(roleId, out role);
            if (result)
            {
                Utility.Debug.LogWarning($"RoleId:{role.RoleId};SessionId:{role.SessionId}离开Level：{LevelId}");
                RoleSendMsgHandler -= role.SendMessage;
                role.TryRemove(typeof(LevelEntity));
            }
            return result;
        }
        /// <summary>
        ///接收到消息后直接存储，不考虑顺序 
        /// </summary>
#if SERVER
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
        }
#else
        public void OnCommandS2C(IDataContract data)
        {
            roleSendMsgHandler?.Invoke(data);
        }
#endif
        public void OnRefresh()
        {
#if SERVER
            if (!Available)
                return;
            Utility.Debug.LogInfo($"LevelId:{LevelId}刷新发送,PlayerCout:{PlayerCount}");
            var result = roleInputCmdDict.TryGetValue(currentTick, out var roleCmds);
            if (result)
            {
                InputSet.InputDict = roleCmds;
                InputSet.Tick = (int)currentTick;
                opData.DataContract = InputSet;
                roleSendMsgHandler?.Invoke(opData);
                //若当前帧发送成功，则移除上一个逻辑帧数据；服务器当前不存储数据，仅负责转发；
                roleInputCmdDict.Remove(currentTick--);
            }
            else {
                InputSet.InputDict = null;
                InputSet.Tick = (int)currentTick;
                opData.DataContract = InputSet;
                roleSendMsgHandler?.Invoke(opData);
            }
            currentTick++;
#else

#endif
        }
        public void Clear()
        {
            Utility.Debug.LogWarning($"Level:{LevelId}无玩家，Clear");
            roleDict.Clear();
            this.LevelId = 0;
            this.Available = false;
            roleInputCmdDict.Clear();
            roleSendMsgHandler = null;
#if SERVER
            InputSet.Clear();
#endif
        }
#if SERVER

        public static LevelEntity Create(int sceneId, params RoleEntity[] peerEntities)
        {
            LevelEntity se = GameManager.ReferencePoolManager.Spawn<LevelEntity>();
            se.OnInit(sceneId);
            int length = peerEntities.Length;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(int sceneId, List<RoleEntity> peerEntities)
        {
            LevelEntity se = GameManager.ReferencePoolManager.Spawn<LevelEntity>();
            se.OnInit(sceneId);
            int length = peerEntities.Count;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static LevelEntity Create(int sceneId)
        {
            LevelEntity se = GameManager.ReferencePoolManager.Spawn<LevelEntity>();
            se.OnInit(sceneId);
            return se;
        }
#endif
    }
}
