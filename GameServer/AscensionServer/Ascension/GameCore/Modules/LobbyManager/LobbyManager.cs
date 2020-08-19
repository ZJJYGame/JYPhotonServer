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
    /// 玩家的大厅管理器
    /// </summary>
    public sealed class LobbyManager:ModuleBase<LobbyManager>
    {
        Dictionary<int, AscensionPeer> peerDict = new Dictionary<int, AscensionPeer>();
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
            else
            {
                peerDict.Add(peerID, peer);
                return true;
            }
        }
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="peerID">peerID</param>
        /// <returns>是否离开成功</returns>
        public bool LeaveLobby(int peerID)
        {
            //AscensionPeer peer;
            //return peerDict.Remove(peerID, out peer);
            return peerDict.Remove(peerID);
        }
        /// <summary>
        /// 是否在大厅中
        /// </summary>
        /// <param name="peerID">id</param>
        /// <returns>是否存在</returns>
        public bool IsInLobby(int peerID)
        {
            return peerDict.ContainsKey(peerID);
        }
        /// <summary>
        /// 在大厅中通过ID查找Peer
        /// </summary>
        /// <param name="peerID">id</param>
        /// <returns>查找到的对象</returns>
        public AscensionPeer GetPeerInLobby(int peerID)
        {
            AscensionPeer peer;
             peerDict.TryGetValue(peerID, out peer);
            return peer;
        }
    }
}
