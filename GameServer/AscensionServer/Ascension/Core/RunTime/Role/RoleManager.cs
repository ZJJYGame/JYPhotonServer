using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AscensionServer
{
    [Module]
    public class RoleManager : Cosmos. Module,IRoleManager
    {
        public int RoleCount { get { return roleDict.Count; } }
        ConcurrentDictionary<int, RoleEntity> roleDict = new ConcurrentDictionary<int, RoleEntity>();


        event Action<OperationData> BroadcastMsg1Param
        {
            add { broadcastMsg1Param += value; }
            remove { broadcastMsg1Param -= value; }
        }
        Action<OperationData> broadcastMsg1Param;

        event Action<byte, Dictionary<byte, object>> BroadcastMsg2Params
        {
            add { broadcastMsg2Params += value; }
            remove{broadcastMsg2Params -= value;}
        }
        Action<byte, Dictionary<byte, object>> broadcastMsg2Params;

        event Action<byte, short, Dictionary<byte, object>> BroadcastMsg3Params
        {
            add { broadcastMsg3Params += value; }
            remove { broadcastMsg3Params -= value; }
        }
        Action<byte, short, Dictionary<byte, object>> broadcastMsg3Params;

        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.LogoffRole, OnPlayerLogoff);
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
                BroadcastMsg3Params += role.SendMessage;
                BroadcastMsg2Params += role.SendMessage;
                BroadcastMsg1Param += role.SendMessage;
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
                BroadcastMsg3Params -= role.SendMessage;
                BroadcastMsg2Params -= role.SendMessage;
                BroadcastMsg1Param -= role.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int roleId, out RoleEntity role)
        {
            var result = roleDict.TryRemove(roleId, out role);
            if (result)
            {
                BroadcastMsg3Params -= role.SendMessage;
                BroadcastMsg2Params -= role.SendMessage;
                BroadcastMsg1Param -= role.SendMessage;
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
                    BroadcastMsg3Params += newRole.SendMessage;
                    BroadcastMsg2Params += newRole.SendMessage;
                    BroadcastMsg1Param += newRole.SendMessage;
                    BroadcastMsg3Params -= oldRole.SendMessage;
                    BroadcastMsg2Params -= oldRole.SendMessage;
                    BroadcastMsg1Param -= oldRole.SendMessage;
                }
                roleDict[roleId] = newRole;
                return true;
            }
            return false;
        }


        /// <summary>
        ///通过roleId发送信息到登录的角色;
        /// </summary>
        public bool SendMessage(int roleId, OperationData message)
        {
            var result = TryGetValue(roleId, out var role);
            if (result)
                role.SendMessage(message);
            return result;
        }
        public bool SendMessage(int roleId, byte opCode, Dictionary<byte, object> userData)
        {
            var result = TryGetValue(roleId, out var role);
            if (result)
                role.SendMessage(opCode, userData);
            return result;
        }
        public bool SendMessage(int roleId, byte opCode,short subCode, Dictionary<byte, object> userData)
        {
            var result = TryGetValue(roleId, out var role);
            if (result)
                role.SendMessage(opCode, subCode,userData);
            return result;
        }


        public void BroadcastMessageToAll(OperationData message)
        {
            broadcastMsg1Param?.Invoke(message);
        }
        /// <summary>
        /// 广播消息到所有登录的角色
        /// </summary>
        public void BroadcastMessageToAll(byte opCode, Dictionary<byte, object> userData)
        {
            broadcastMsg2Params?.Invoke(opCode, userData);
        }
        public void BroadcastMessageToAll(byte opCode,short subCode, Dictionary<byte, object> userData)
        {
            broadcastMsg3Params?.Invoke(opCode, subCode, userData);
        }


        public async Task<bool> SendMessageAsync(int roleId, OperationData message)
        {
            return await Task.Run(() => { return SendMessage(roleId, message); });
        }
        public async Task<bool> SendEventAsync(int roleId, byte opCode, Dictionary<byte, object> userData)
        {
            return await Task.Run(() => { return SendMessage(roleId, opCode, userData); });
        }
        public async Task<bool> SendEventAsync(int roleId, byte opCode,short subCode, Dictionary<byte, object> userData)
        {
            return await Task.Run(() => { return SendMessage(roleId, opCode,subCode, userData); });

        }


        public async Task BroadcastMessageToAllAsync(OperationData message, Action callback = null)
        {
            await Task.Run(() => { broadcastMsg1Param?.Invoke(message); });
            callback?.Invoke();
        }
        public async Task BroadcastMessageToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null)
        {
            await Task.Run(() => { broadcastMsg2Params?.Invoke(opCode, userData); });
            callback?.Invoke();
        }
        public async Task BroadcastMessageToAllAsync(byte opCode, short subCode,Dictionary<byte, object> userData, Action callback = null)
        {
            await Task.Run(() => { broadcastMsg3Params?.Invoke(opCode, subCode,userData); });
            callback?.Invoke();
        }
        void OnPlayerLogoff(int sessionId, OperationData opData)
        {
            var roleEntity= opData.DataMessage as RoleEntity;
            if (roleEntity != null)
            {
                TryRemove(roleEntity.RoleId);
            }
        }
    }
}

