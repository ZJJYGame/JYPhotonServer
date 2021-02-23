using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
using System;
using System.Threading.Tasks;
using Protocol;
using System.Collections.Concurrent;

namespace AscensionServer
{
    /// <summary>
    /// ��ɫ��������
    /// keyΪRoleId����PlayerId
    /// </summary>
    [Module]
    public class RoleManager : Module,IRoleManager
    {
        public int RoleCount { get { return roleDict.Count; } }
        ConcurrentDictionary<int, RoleEntity> roleDict = new ConcurrentDictionary<int, RoleEntity>();
        ConcurrentQueue<RoleEntity> playerPoolQueue = new ConcurrentQueue<RoleEntity>();
        /// <summary>
        /// �㲥�¼���Ϣ ;
        /// </summary>
        event Action<byte, Dictionary<byte, object>> BroadcastEvent
        {
            add { broadcastEvent += value; }
            remove{broadcastEvent -= value;}
        }
        /// <summary>
        /// �㲥��ͨ��Ϣ;
        /// </summary>
        event Action<OperationData> BroadcastMessage
        {
            add { broadcastMessage += value; }
            remove{broadcastMessage -= value;}
        }
        Action<byte, Dictionary<byte, object>> broadcastEvent;
        Action<OperationData> broadcastMessage;
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.PORT_CHAT, OnChatMessage);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLYAER_LOGOFF, OnPlayerLogoff);
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
                BroadcastEvent += role.SendEvent;
                BroadcastMessage += role.SendMessage;
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
                BroadcastEvent -= role.SendEvent;
                BroadcastMessage -= role.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int roleId, out RoleEntity role)
        {
            var result = roleDict.TryRemove(roleId, out role);
            if (result)
            {
                BroadcastEvent -= role.SendEvent;
                BroadcastMessage -= role.SendMessage;
            }
            return result;
        }
        public bool TryUpdate(int roleId, RoleEntity newRole, RoleEntity comparsionRole)
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
                }
                roleDict[roleId] = newRole;
                return true;
            }
            return false;
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
        void OnPlayerLogoff(OperationData opData)
        {
            var roleEntity= opData.DataMessage as RoleEntity;
            if (roleEntity != null)
            {
                TryRemove(roleEntity.RoleId);
            }
        }
    }
}

