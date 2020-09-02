using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// photon所有登录的peer都存储在此；
    /// 其余各个模块都是从此通过Conv取得Peer对象；
    /// 各个模块本身不存储对象，仅做消息转发
    /// </summary>
    public class PeerManager : Module<PeerManager>,IKeyValue<uint, PeerEntity>
    {
        ConcurrentDictionary<uint, PeerEntity> peerDict;
        public override void OnInitialization()
        {
            peerDict = new ConcurrentDictionary<uint, PeerEntity>();
        }
        public bool TryAdd(uint conv, PeerEntity peer)
        {
            return peerDict.TryAdd(conv, peer);
        }
        public bool TryRemove(uint conv)
        {
            PeerEntity peer;
            return peerDict.TryRemove(conv, out peer);
        }
        public bool ContainsKey(uint conv)
        {
            return peerDict.ContainsKey(conv);
        }
        /// <summary>
        /// 将指定键的现有值与指定值进行比较，如果相等，则用第三个值更新该键。
        /// </summary>
        public bool TryUpdate(uint conv, PeerEntity newPeer, PeerEntity comparisonPeer)
        {
            return peerDict.TryUpdate(conv, newPeer, comparisonPeer);
        }
        public bool TryGetValue(uint conv,out PeerEntity peer)
        {
            return peerDict.TryGetValue(conv, out peer);
        }
    }
}
