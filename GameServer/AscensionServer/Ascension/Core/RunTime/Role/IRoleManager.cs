﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;

namespace AscensionServer
{
    public interface IRoleManager:IModuleManager
    {
        /// <summary>
        /// Id为RoleId
        /// </summary>
        event Action<int> OnRoleLogoff;
        /// <summary>
        /// Id为RoleId
        /// </summary>
        event Action<int> OnRoleLogOn;
        int RoleCount { get; }
        bool ContainsKey(int roleId);
        bool TryAdd(int roleId, RoleEntity role);
        bool TryGetValue(int roleId, out RoleEntity role);
        bool TryRemove(int roleId);
        bool TryRemove(int roleId, out RoleEntity role);
        bool TryUpdate(int roleId, RoleEntity newRole, RoleEntity comparsionRole);
        /// <summary>
        ///通过roleId发送信息到登录的角色;
        /// </summary>
        bool SendMessage(int roleId, OperationData message);
        bool SendMessage(int roleId, byte opCode, Dictionary<byte, object> userData);
        bool SendMessage(int roleId, byte opCode, short subCode, Dictionary<byte, object> userData);


        void BroadcastMessageToAll(OperationData message);
        /// <summary>
        /// 广播消息到所有登录的角色
        /// </summary>
        void BroadcastMessageToAll(byte opCode, Dictionary<byte, object> userData);
        void BroadcastMessageToAll(byte opCode, short subCode, Dictionary<byte, object> userData);


        Task<T> GetRoleDataAsync<T>(int roleId) where T : DataTransferObject, new();
        Task<T[]> GetRoleDatasAsync<T>(int[] roleIds) where T : DataTransferObject, new();

        Task<bool> SendMessageAsync(int roleId, OperationData message);
        Task<bool> SendEventAsync(int roleId, byte opCode, Dictionary<byte, object> userData);
        Task<bool> SendEventAsync(int roleId, byte opCode, short subCode, Dictionary<byte, object> userData);


        Task BroadcastMessageToAllAsync(OperationData message, Action callback = null);
        Task BroadcastMessageToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null);
        Task BroadcastMessageToAllAsync(byte opCode, short subCode, Dictionary<byte, object> userData, Action callback = null);
    }
}


