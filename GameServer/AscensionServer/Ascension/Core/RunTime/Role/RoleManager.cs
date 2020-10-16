using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
using System;
using System.Threading.Tasks;
using Protocol;

namespace AscensionServer
{
    /// <summary>
    /// ��ɫ��������
    /// keyΪRoleId����PlayerId
    /// </summary>
    [CustomeModule]
    public class RoleManager : Module<RoleManager>
    {
        public int RoleCount { get { return roleDict.Count; } }
        Dictionary<int, IRoleEntity> roleDict = new Dictionary<int, IRoleEntity>();
        Queue<IRoleEntity> playerPoolQueue = new Queue<IRoleEntity>();
        /// <summary>
        /// �㲥�¼���Ϣ ;
        /// </summary>
        event Action<byte, Dictionary<byte, object>> BroadcastEvent
        {
            add { broadcastEvent += value; }
            remove
            {
                try { broadcastEvent -= value; }
#if SERVER
                catch (Exception e) { Utility.Debug.LogError($"�޷��Ƴ�������Ϣ��ί��:{e}"); }
#else
                catch (Exception e) { Utility.DebugError($"�޷��Ƴ�������Ϣ��ί��:{e}"); }
#endif
            }
        }
        /// <summary>
        /// �㲥��ͨ��Ϣ;
        /// </summary>
        event Action<OperationData> BroadcastMessage
        {
            add { broadcastMessage += value; }
            remove
            {
                try { broadcastMessage -= value; }
#if SERVER
                catch (Exception e) { Utility.Debug.LogError($"�޷��Ƴ�������Ϣ��ί��:{e}"); }
#else
                catch (Exception e) { Utility.DebugError($"�޷��Ƴ�������Ϣ��ί��:{e}"); }
#endif
            }
        }
        Action<byte, Dictionary<byte, object>> broadcastEvent;
        Action<OperationData> broadcastMessage;
#if !SERVER
        Action refreshHandler;
        event Action RefreshHandler
        {
            add { refreshHandler += value; }
            remove
            {
                try{refreshHandler -= value;}
                catch (global::System.Exception e){Utility.DebugError(e);}
            }
        }
#endif
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.PORT_CHAT, OnChatMessage);
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
                BroadcastEvent += role.SendEvent;
                BroadcastMessage += role.SendMessage;
#if !SERVER
                RefreshHandler += role.OnRefresh;
#endif
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
                BroadcastEvent -= role.SendEvent;
                BroadcastMessage -= role.SendMessage;
#if !SERVER
                RefreshHandler -= role.OnRefresh;
#endif
            }
            return result;
        }
        public bool TryRemove(int roleId, out IRoleEntity role)
        {
            var result = roleDict.Remove(roleId, out role);
            if (result)
            {
                BroadcastEvent -= role.SendEvent;
                BroadcastMessage -= role.SendMessage;
#if !SERVER
                RefreshHandler -= role.OnRefresh;
#endif
            }
            return result;
        }
        public bool TryUpdate(int roleId, IRoleEntity newRole, IRoleEntity comparsionRole)
        {
            if (!newRole.Equals(comparsionRole))
                return false;
            if (roleDict.ContainsKey(roleId))
            {

                var oldRole = roleDict[roleId];
                {
                    BroadcastEvent += newRole.SendEvent;
                    BroadcastMessage += newRole.SendMessage;
                    BroadcastEvent -= oldRole.SendEvent;
                    BroadcastMessage -= oldRole.SendMessage;
#if !SERVER
                    RefreshHandler -= oldRole.OnRefresh;
                    RefreshHandler += newRole.OnRefresh;
#endif
                }
                roleDict[roleId] = newRole;
                return true;
            }
            return false;
        }
        public override void OnRefresh()
        {
#if !SERVER
            refreshHandler?.Invoke();
#endif
        }
        public bool SendEvent(int roleId, byte opCode, Dictionary<byte, object> userData)
        {
            var result = TryGetValue(roleId, out var role);
            if (result)
                role.SendEvent(opCode, userData);
            return result;
        }
        /// <summary>
        /// ͬ���㲥�¼�(EVENT)��
        /// �˷����������������Available��peer���������Ϣ�㲥��
        /// </summary>
        /// <param name="userData">�û��Զ�������</param>
        public void BroadcastEventToAll(byte opCode, Dictionary<byte, object> userData)
        {
            broadcastEvent?.Invoke(opCode, userData);
        }
        /// <summary>
        ///������Ϣ�������SessionId 
        /// </summary>
        public bool SendMessage(int roleId, OperationData message)
        {
            var result = TryGetValue(roleId, out var role);
            if (result)
                role.SendMessage(message);
            return result;
        }
        /// <summary>
        /// ͨ���㲥��ͨ��Ϣ(MSG)��
        /// </summary>
        /// <param name="message">��ͨ��Ϣ</param>
        public void BroadcastMessageToAll(OperationData message)
        {
            broadcastMessage?.Invoke(message);
        }
        /// <summary>
        ///�첽������Ϣ�������SessionId 
        /// </summary>
        public async Task<bool> SendMessageAsync(int roleId, OperationData message)
        {
            return await Task.Run(() => { return SendMessage(roleId, message); });
        }
        /// <summary>
        ///�첽�㲥��Ϣ�������sessionId 
        /// </summary>
        public async Task<bool> SendEventAsync(int roleId, byte opCode, Dictionary<byte, object> userData)
        {
            return await Task.Run(() => { return SendEvent(roleId, opCode, userData); });
        }
        /// <summary>
        /// �첽�㲥�¼���Ϣ(EVENT)��
        /// </summary>
        /// <param name="userData">�û��Զ�������</param>
        /// <param name="callback">�㲥������Ļص�</param>
        /// <returns>�߳�Task</returns>
        public async Task BroadcastEventToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null)
        {
            await Task.Run(() => { broadcastEvent?.Invoke(opCode, userData); });
            callback?.Invoke();
        }
        /// <summary>
        /// �첽�㲥��ͨ��Ϣ(MSG)��
        /// </summary>
        /// <param name="message">��ͨ��Ϣ</param>
        /// <param name="callback">��Ϣ�㲥��ɺ�Ļص�</param>
        /// <returns></returns>
        public async Task BroadcastMessageToAllAsync(OperationData message, Action callback = null)
        {
            await Task.Run(() => { broadcastMessage?.Invoke(message); });
            callback?.Invoke();
        }
        void OnChatMessage(OperationData opData)
        {

        }
    }
}