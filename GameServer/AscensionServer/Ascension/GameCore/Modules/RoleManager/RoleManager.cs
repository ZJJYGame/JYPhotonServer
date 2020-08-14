using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 登录管理，缓存所有登录玩家的peer信息
    /// </summary>
    public sealed class RoleManager:ModuleBase<RoleManager>
    {
        ConcurrentDictionary<int, AscensionPeer> loggedPeerDict = new ConcurrentDictionary<int, AscensionPeer>();
        /// <summary>
        /// 将登录成功的Peer进行缓存
        /// </summary>
        /// <param name="peerID">id</param>
        /// <param name="peer">peer</param>
        /// <returns>是否登录成功</returns>
        public bool Loggin(int peerID, AscensionPeer peer)
        {
            return loggedPeerDict.TryAdd(peerID, peer);
        }
        /// <summary>
        /// 下线，移除peer的缓存
        /// </summary>
        /// <param name="peerID">id</param>
        /// <returns>下线成功否</returns>
        public bool Logoff(int peerID)
        {
            AscensionPeer peer;
            return loggedPeerDict.TryRemove(peerID, out peer);
        }
        /// <summary>
        /// 登录互斥检测；
        /// 如果存在登录，则移除先前的Peer，执行OnTermination 方法；
        /// </summary>
        /// <param name="peerID"></param>
        /// <param name="peer"></param>
        public void ExclusionCheck(int peerID, AscensionPeer peer)
        {
            if (loggedPeerDict.ContainsKey(peerID))
            {
                AscensionPeer latestPeer;
                loggedPeerDict.TryRemove(peerID, out latestPeer);
                latestPeer.OnTermination();
            }
        }
        /// <summary>
        /// 通过ID获取Peer
        /// </summary>
        /// <param name="peerID">id</param>
        /// <returns>peer对象</returns>
        public AscensionPeer GetPeer(int peerID)
        {
            AscensionPeer peer;
            loggedPeerDict.TryGetValue(peerID, out peer);
            return peer;
        }
    }
}
