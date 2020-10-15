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
    public class SceneEntity :  IReference,IRefreshable
    {
        public int SceneId { get; private set; }
        public bool Available { get; private set; }
        Dictionary <int, IRoleEntity> peerDict;
        Action<object> roleSendMsgHandler;
        event Action<object> RoleSendMsgHandler 
        {
            add { roleSendMsgHandler += value; }
            remove
            {
                try{roleSendMsgHandler -= value;}
                catch (Exception e){Utility.Debug.LogError(e);}
            }
        }
        long currentTick;
        public Dictionary<long, Dictionary<int, C2SInput>> roleInputCmdDict 
            = new Dictionary<long, Dictionary<int, C2SInput>>();
        S2CInput InputSet = new S2CInput();
        public int PlayerCount { get { return peerDict.Count; } }
        public SceneEntity()
        {
            peerDict = new Dictionary<int, IRoleEntity>();
        }
        public void OnInit(int sceneId)
        {
            this.SceneId = sceneId;
            this.Available = true;
            this.InputSet.EntityContainerId = sceneId;
        }
        public bool ContainsKey(int  roleId)
        {
            return peerDict.ContainsKey(roleId);
        }
        public bool TryAdd(int roleId, IRoleEntity role)
        {
            var result= peerDict.TryAdd(roleId, role);
            if (result)
            {
                RoleSendMsgHandler += role.SendMessage;
            }
            return result;
        }
        public bool TryAdd(IRoleEntity role)
        {
            var result = peerDict.TryAdd(role.RoleId, role);
            if (result)
            {
                RoleSendMsgHandler += role.SendMessage;
            }
            return result;
        }
        public bool TryGetValue(int roleId, out IRoleEntity role)
        {
            return peerDict.TryGetValue(roleId, out role);
        }
        public bool TryRemove(int roleId)
        {
            var result= peerDict.Remove(roleId, out var role );
            if (result)
            {
                RoleSendMsgHandler -= role.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int roleId, out IRoleEntity role)
        {
            var result= peerDict.Remove(roleId, out role);
            if (result)
            {
                RoleSendMsgHandler -= role.SendMessage;
            }
            return result;
        }
        /// <summary>
        ///接收到消息后直接存储，不考虑顺序 
        /// </summary>
        public void OnPlayerInput(IDataContract data )
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
        public void OnRefresh()
        {
            var result= roleInputCmdDict.TryGetValue(currentTick,out var roleCmds);
            if (result)
            {
                InputSet.InputDict = roleCmds;
                InputSet.Tick = (int)currentTick;
                roleSendMsgHandler?.Invoke(InputSet);
                //若当前帧发送成功，则移除上一个逻辑帧数据；服务器当前不存储数据，仅负责转发；
                roleInputCmdDict.Remove(currentTick--);
            }
            currentTick++;
        }
        public void Clear()
        {
            peerDict.Clear();
            this.SceneId = 0;
            this.Available = false;
        }
        public static SceneEntity Create(int sceneId, params IRoleEntity[] peerEntities)
        {
            SceneEntity se = new SceneEntity();
            se.OnInit(sceneId);
            int length = peerEntities.Length;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static SceneEntity Create(int sceneId, List<IRoleEntity> peerEntities)
        {
            SceneEntity se = new SceneEntity();
            se.OnInit(sceneId);
            int length = peerEntities.Count;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static SceneEntity Create(int sceneId)
        {
            SceneEntity se = new SceneEntity();
            se.OnInit(sceneId);
            return se;
        }

    }
}
