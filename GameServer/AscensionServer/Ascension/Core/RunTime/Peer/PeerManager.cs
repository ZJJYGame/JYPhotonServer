using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using AscensionProtocol;
namespace AscensionServer
{
    /// <summary>
    /// <see cref="AscensionPeer"/>
    /// photon所有登录的peer都存储在此；
    /// SessionId在具体的Peer中，由服务器分配；
    /// 其余各个模块都是从此通过SessionID取得Peer对象；
    /// </summary>
    [CustomeModule]
    public class PeerManager : Module<PeerManager>, IKeyValue<int, IPeerAgent>
    {
        /// <summary>
        /// 广播事件消息 ;
        /// </summary>
        public event Action<byte, object> BroadcastEventMessage
        {
            add { broadcastEventMessage += value; }
            remove
            {
                try { broadcastEventMessage -= value; }
                catch (Exception e) { Utility.Debug.LogError($"无法移除发送消息的委托:{e}"); }
            }
        }
        /// <summary>
        /// 广播普通消息;
        /// </summary>
        public event Action<object> BroadcastMessage
        {
            add { broadcastMessage += value; }
            remove
            {
                try { broadcastMessage -= value; }
                catch (Exception e) { Utility.Debug.LogError($"无法移除发送消息的委托:{e}"); }
            }
        }
        Action<byte, object> broadcastEventMessage;
        Action<object> broadcastMessage;
        ConcurrentDictionary<int, IPeerAgent> peerDict;
        public override void OnInitialization()
        {
            peerDict = new ConcurrentDictionary<int, IPeerAgent>();
        }
        public bool TryAdd(IPeerAgent peer)
        {
            var result = peerDict.TryAdd(peer.SessionId, peer);
            if (result)
            {
                BroadcastEventMessage += peer.SendEventMessage;
                BroadcastMessage += peer.SendMessage;
            }
            return result;
        }
        public bool TryAdd(int sessionId, IPeerAgent peer)
        {
            var result = peerDict.TryAdd(sessionId, peer);
            if (result)
            {
                BroadcastEventMessage += peer.SendEventMessage;
                BroadcastMessage += peer.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int sessionId)
        {
            IPeerAgent peer;
            var result = peerDict.TryRemove(sessionId, out peer);
            if (result)
            {
                BroadcastEventMessage -= peer.SendEventMessage;
                BroadcastMessage -= peer.SendMessage;
            }
            return result;
        }
        public bool TryRemove(int sessionId, out IPeerAgent peer)
        {
            var result = peerDict.TryRemove(sessionId, out peer);
            if (result)
            {
                BroadcastEventMessage -= peer.SendEventMessage;
                BroadcastMessage -= peer.SendMessage;
            }
            return result;
        }
        public bool ContainsKey(int sessionId)
        {
            return peerDict.ContainsKey(sessionId);
        }
        /// <summary>
        /// 将指定键的现有值与指定值进行比较，如果相等，则用第三个值更新该键。
        /// </summary>
        public bool TryUpdate(int sessionId, IPeerAgent newPeer, IPeerAgent comparisonPeer)
        {
            var result = peerDict.TryUpdate(sessionId, newPeer, comparisonPeer);
            if (result)
            {
                BroadcastEventMessage -= comparisonPeer.SendEventMessage;
                BroadcastEventMessage += newPeer.SendEventMessage;
                BroadcastMessage -= comparisonPeer.SendMessage;
                BroadcastMessage += newPeer.SendMessage;
            }
            return result;
        }
        public bool TryGetValue(int sessionId, out IPeerAgent peer)
        {
            return peerDict.TryGetValue(sessionId, out peer);
        }
        /// <summary>
        /// 同步广播事件；
        /// 此方法会对所有在线且Available的peer对象进行消息广播；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void BroadcastEventMessageToAll(byte opCode, object userData)
        {
            broadcastEventMessage?.Invoke(opCode, userData);
        }
        /// <summary>
        /// 通过广播普通消息；
        /// </summary>
        /// <param name="message">普通消息</param>
        public void BroadcastMessageToAll(object message)
        {
            broadcastMessage?.Invoke(message);
        }
        /// <summary>
        /// 异步广播事件消息；
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        public async Task BroadcastEventMessageToAllAsync(byte opCode, object userData, Action callback = null)
        {
            await Task.Run(() => { broadcastEventMessage?.Invoke(opCode, userData); });
            callback?.Invoke();
        }
        /// <summary>
        /// 异步广播普通消息；
        /// </summary>
        /// <param name="message">普通消息</param>
        /// <param name="callback">消息广播完成后的回调</param>
        /// <returns></returns>
        public async Task BroadcastMessageToAllAsync(object message, Action callback = null)
        {
            await Task.Run(() => { broadcastMessage?.Invoke(message); });
            callback?.Invoke();
        }
    }
}
