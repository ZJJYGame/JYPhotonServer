using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;

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
        ///发送事件(EVENT)到具体的sessionId； 
        ///若不存在session对象，则不发送，并返回false；
        /// </summary>
        bool SendEvent(int sessionId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 同步广播事件(EVENT)；
        /// 此方法会对所有在线且Available的peer对象进行消息广播；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        void BroadcastEventToAll(byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        ///发送消息到具体的SessionId 
        /// </summary>
        bool SendMessage(int sessionId, OperationData message);
        /// <summary>
        /// 通过广播消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        void BroadcastMessageToAll(OperationData message);
        /// <summary>
        ///异步发送消息到具体的SessionId 
        /// </summary>
        Task<bool> SendMessageAsync(int sessionId, OperationData message);
        /// <summary>
        ///异步广播消息到具体的sessionId 
        /// </summary>
        Task<bool> SendEventAsync(int sessionId, byte opCode, Dictionary<byte, object> userData);
        /// <summary>
        /// 异步广播事件消息(EVENT)；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        Task BroadcastEventToAllAsync(byte opCode, Dictionary<byte, object> userData, Action callback = null);
        /// <summary>
        /// 异步广播消息(MSG)；
        /// </summary>
        /// <param name="message">普通消息</param>
        /// <param name="callback">消息广播完成后的回调</param>
        /// <returns></returns>
        Task BroadcastMessageToAllAsync(OperationData message, Action callback = null);
    }
}


