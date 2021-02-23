using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    public interface IRoleManager:IModuleManager
    {
        int RoleCount { get; }
        bool ContainsKey(int roleId);
        bool TryAdd(int roleId, RoleEntity role);
        bool TryGetValue(int roleId, out RoleEntity role);
        bool TryRemove(int roleId);
        bool TryRemove(int roleId, out RoleEntity role);
        bool TryUpdate(int roleId, RoleEntity newRole, RoleEntity comparsionRole);
        bool SendEvent(int roleId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 同步广播事件(EVENT)；
        /// 此方法会对所有在线且Available的peer对象进行消息广播；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        void BroadcastEventToAll(byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        ///发送消息到具体的SessionId 
        /// </summary>
        bool SendMessage(int roleId, OperationData message);
        /// <summary>
        /// 通过广播普通消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        void BroadcastMessageToAll(OperationData message);
        /// <summary>
        ///异步发送消息到具体的SessionId 
        /// </summary>
        Task<bool> SendMessageAsync(int roleId, OperationData message);
        /// <summary>
        ///异步广播消息到具体的sessionId 
        /// </summary>
        Task<bool> SendEventAsync(int roleId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 异步广播事件消息(EVENT)；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        Task BroadcastEventToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null);
        /// <summary>
        /// 异步广播普通消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        /// <param name="callback">消息广播完成后的回调</param>
        /// <returns></returns>
        Task BroadcastMessageToAllAsync(OperationData message, Action callback = null);
    }
}


