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
    public class LevelEntity : IReference, IRefreshable
    {
        public int SceneId { get; private set; }
        public bool Available { get; private set; }
        Dictionary<int, IRoleEntity> roleDict;
        Action<object> roleSendMsgHandler;
        event Action<object> RoleSendMsgHandler
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
        public int PlayerCount { get { return roleDict.Count; } }
        public LevelEntity()
        {
            roleDict = new Dictionary<int, IRoleEntity>();
        }
        public void OnInit(int sceneId)
        {
            this.SceneId = sceneId;
            this.Available = true;
#if SERVER
            this.InputSet.EntityContainerId = sceneId;
#endif
        }
        public bool ContainsKey(int roleId)
        {
            return roleDict.ContainsKey(roleId);
        }
        public bool TryAdd(int roleId, IRoleEntity role)
        {
            var result = roleDict.TryAdd(roleId, role);
            if (result)
            {
                RoleSendMsgHandler += role.SendMessage;
            }
            return result;
        }
        public bool TryAdd(IRoleEntity role)
        {
            var result = roleDict.TryAdd(role.RoleId, role);
            if (result)
            {
                RoleSendMsgHandler += role.SendMessage;
            }
            return result;
        }
        public bool TryGetValue(int roleId, out IRoleEntity role)
        {
            return roleDict.TryGetValue(roleId, out role);
        }
        public bool TryRemove(int roleId)
        {
            var result = roleDict.Remove(roleId, out var role);
            if (result)
            {
                RoleSendMsgHandler -= role.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int roleId, out IRoleEntity role)
        {
            var result = roleDict.Remove(roleId, out role);
            if (result)
            {
                RoleSendMsgHandler -= role.SendMessage;
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
            var result = roleInputCmdDict.TryGetValue(currentTick, out var roleCmds);
            if (result)
            {
                InputSet.InputDict = roleCmds;
                InputSet.Tick = (int)currentTick;
                roleSendMsgHandler?.Invoke(InputSet);
                //若当前帧发送成功，则移除上一个逻辑帧数据；服务器当前不存储数据，仅负责转发；
                roleInputCmdDict.Remove(currentTick--);
            }
            currentTick++;
#else

#endif
        }
        public void Clear()
        {
            roleDict.Clear();
            this.SceneId = 0;
            this.Available = false;
            roleInputCmdDict.Clear();
            roleSendMsgHandler = null;
#if SERVER
            InputSet.Clear();
#endif
        }
#if SERVER

        public static LevelEntity Create(int sceneId, params IRoleEntity[] peerEntities)
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
        public static LevelEntity Create(int sceneId, List<IRoleEntity> peerEntities)
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
