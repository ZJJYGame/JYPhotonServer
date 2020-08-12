using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 大厅管理器
    /// </summary>
    public class LobbyManager:ConcurrentSingleton<LobbyManager>
    {
        ConcurrentDictionary<int, AscensionPeer> peerDict = new ConcurrentDictionary<int, AscensionPeer>();
        /// <summary>
        /// 进入大厅
        /// </summary>
        /// <param name="peerID">PeerID</param>
        /// <param name="peer">peer对象</param>
        /// <returns>是否进入成功</returns>
        public bool EnterLobby(int peerID,AscensionPeer peer)
        {
            if (peerDict.ContainsKey(peerID))
                return false;
            return peerDict.TryAdd(peerID, peer);
        }
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="peerID">peerID</param>
        /// <returns>是否离开成功</returns>
        public bool LeaveLobby(int peerID)
        {
            AscensionPeer peer;
            return peerDict.TryRemove(peerID, out peer);
        }
    }
}
