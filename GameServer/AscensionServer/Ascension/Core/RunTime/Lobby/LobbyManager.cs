using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    /// <summary>
    /// 玩家的大厅管理器
    /// </summary>
    public sealed class LobbyManager:Module<LobbyManager>
    {
        HashSet<uint> peerSet=new HashSet<uint>();
        public bool TryAdd(uint conv)
        {
            return peerSet.Add(conv);
        }
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="conv">peerID</param>
        /// <returns>是否离开成功</returns>
        public bool TryRemove(uint conv)
        {
            return peerSet.Remove(conv);
        }
        /// <summary>
        /// 是否在大厅中
        /// </summary>
        /// <param name="conv">id</param>
        /// <returns>是否存在</returns>
        public bool Contains(uint conv)
        {
            return peerSet.Contains(conv);
        }
        /// <summary>
        /// 在大厅中通过ID查找Peer
        /// </summary>
        /// <param name="peerID">id</param>
        /// <returns>查找到的对象</returns>
        public bool TryGetValue(uint conv,out PeerEntity peer)
        {
            peer = default;
            if (!Contains(conv))
                return false;
            return PeerManager.Instance.TryGetValue(conv, out peer);
        }
    }
}
