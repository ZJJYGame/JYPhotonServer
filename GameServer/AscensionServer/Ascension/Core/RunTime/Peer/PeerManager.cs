using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace AscensionServer
{
    /// <summary>
    /// photon所有登录的peer都存储在此；
    /// 其余各个模块都是从此通过Conv取得Peer对象；
    /// 各个模块本身不存储对象，仅做消息转发
    /// </summary>
    public class PeerManager : Module<PeerManager>, IKeyValue<long, PeerEntity>
    {
        /// <summary>
        /// 广播消息 
        /// </summary>
        Action<object> broadcastEvent;
        ConcurrentDictionary<long, PeerEntity> peerDict;
        public override void OnInitialization()
        {
            peerDict = new ConcurrentDictionary<long, PeerEntity>();
        }
        public bool TryAdd(long sessionId, PeerEntity peer)
        {
            var result = peerDict.TryAdd(sessionId, peer);
            if (result)
            {
                broadcastEvent += peer.ClientPeer.SendEventMessage;
            }
            return result;
        }
        public bool TryRemove(long sessionId)
        {
            PeerEntity peer;
            var result= peerDict.TryRemove(sessionId, out peer);
            if (result)
            {
                try
                {
                    broadcastEvent -= peer.ClientPeer.SendEventMessage;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"无法移除发送消息的委托:{peer.ClientPeer.Handle.ToString()},{e}");
                }
            }
            return result;
        }
        public bool ContainsKey(long sessionId)
        {
            return peerDict.ContainsKey(sessionId);
        }
        /// <summary>
        /// 将指定键的现有值与指定值进行比较，如果相等，则用第三个值更新该键。
        /// </summary>
        public bool TryUpdate(long sessionId, PeerEntity newPeer, PeerEntity comparisonPeer)
        {
            var result = peerDict.TryUpdate(sessionId, newPeer, comparisonPeer);
            if (result)
            {
                try
                {
                    broadcastEvent -= comparisonPeer.ClientPeer.SendEventMessage;
                    broadcastEvent += newPeer.ClientPeer.SendEventMessage;
                }
                catch (Exception e)
                {
                    Utility.Debug.LogError($"无法更新消息发送委托:{e}");
                }
            }
            return result;
        }
        public bool TryGetValue(long sessionId, out PeerEntity peer)
        {
            return peerDict.TryGetValue(sessionId, out peer);
        }
        /// <summary>
        /// 同步广播事件
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void BroadcastEvent(object userData)
        {
            broadcastEvent?.Invoke(userData);
        }
        /// <summary>
        /// 异步广播事件
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="callback">广播结束后的回调</param>
        /// <returns>线程Task</returns>
        public async Task BroadcastEventAsync(object userData,Action callback=null)
        {
            await Task.Run(() => { broadcastEvent?.Invoke(userData); callback?.Invoke(); });
        }
    }
}
