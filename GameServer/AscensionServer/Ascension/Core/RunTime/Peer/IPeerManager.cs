using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public interface IPeerManager : IModuleManager
    {
        bool TryAdd(IPeerEntity peer);
        bool TryAdd(int sessionId, IPeerEntity peer);
        bool TryRemove(int sessionId);
        bool TryRemove(int sessionId, out IPeerEntity peer);
        bool ContainsKey(int sessionId);
        /// <summary>
        /// 将指定键的现有值与指定值进行比较，如果相等，则用第三个值更新该键。
        /// </summary>
        bool TryUpdate(int sessionId, IPeerEntity newPeer, IPeerEntity comparisonPeer);
        bool TryGetValue(int sessionId, out IPeerEntity peer);

        /// <summary>
        ///发送消息到具体的SessionId 
        /// </summary>
        bool SendMessage(int sessionId, OperationData message);
        /// <summary>
        /// 同步方法；
        /// 通过广播消息(MSG)；
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否能够发送</returns>
        bool SendMessage(int sessionId, byte opCode, short subCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 同步方法；
        /// 通过广播消息(MSG)；
        ///若不存在session对象，则不发送，并返回false；
        /// </summary>
        bool SendMessage(int sessionId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 通过广播消息(MSG)；
        /// 此方法会对所有在线且Available的peer对象进行消息广播；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        void BroadcastMessageToAll(byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 同步方法；
        /// 通过广播消息(MSG)；
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="userData">用户自定义数据</param>
        void BroadcastMessageToAll(byte opCode, short subCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 同步方法；
        /// 通过广播消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        void BroadcastMessageToAll(OperationData message);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        /// <param name="message">普通消息</param>
        /// <returns>线程Task</returns>
        Task<bool> SendMessageAsync(int sessionId, OperationData message);
        /// <summary>
        ///异步广播消息到具体的sessionId 
        /// </summary>
        Task<bool> SendMessageAsync(int sessionId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="sessionId">会话Id</param>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>线程Task</returns>
        Task<bool> SendMessageAsync(int sessionId, byte opCode, short subCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        /// <param name="callback">消息广播完成后的回调</param>
        /// <returns>线程Task</returns>
        Task BroadcastMessageToAllAsync(OperationData message, Action callback = null);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        Task BroadcastMessageToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        Task BroadcastMessageToAllAsync(byte opCode, short subCode, Dictionary<byte, object> userData, Action callback = null);
    }
}


