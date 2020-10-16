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
        HashSet<int> peerSet=new HashSet<int>();
        public bool TryAdd(int sessionId)
        {
            return peerSet.Add(sessionId);
        }
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="sessionId">peerID</param>
        /// <returns>是否离开成功</returns>
        public bool TryRemove(int sessionId)
        {
            return peerSet.Remove(sessionId);
        }
        /// <summary>
        /// 是否在大厅中
        /// </summary>
        /// <param name="sessionId">id</param>
        /// <returns>是否存在</returns>
        public bool Contains(int sessionId)
        {
            return peerSet.Contains(sessionId);
        }
        /// <summary>
        /// 在大厅中通过ID查找Peer
        /// </summary>
        /// <param name="sessionId">id</param>
        /// <returns>查找到的对象</returns>
        public bool TryGetValue(int sessionId, out IPeerEntity peer)
        {
            peer = default;
            if (!Contains(sessionId))
                return false;
            return PeerManager.Instance.TryGetValue(sessionId, out peer);
        }
    }
}
