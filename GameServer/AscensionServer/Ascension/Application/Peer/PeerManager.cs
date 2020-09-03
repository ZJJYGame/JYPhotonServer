using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
        IPeerMessageProvider peerMessageProvider;
        public void SetProvider(IPeerMessageProvider provider)
        {
            peerMessageProvider = provider;
        }
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
        public async Task BroadcastEvent(object userData)
        {
            await Task.Run(() => broadcastEvent?.Invoke(userData));
        }
    }
}
