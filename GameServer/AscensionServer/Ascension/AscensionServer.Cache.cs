using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Threads;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
namespace AscensionServer
{
    public partial class AscensionServer:ApplicationBase
    {
        /// <summary>
        /// 已经连接但是未登录的客户端对象容器
        /// </summary>
        HashSet<AscensionPeer> connectedPeerHashSet = new HashSet<AscensionPeer>();
        public HashSet<AscensionPeer> ConnectedPeerHashSet { get { return connectedPeerHashSet; } set { connectedPeerHashSet = value; } }

        Cache<AscensionPeer> loggedPeerCache = new Cache<AscensionPeer>();
        public Cache<AscensionPeer> LoggedPeerCache { get { return loggedPeerCache; } }
        public void Login(AscensionPeer peer)
        {
            var result = loggedPeerCache.Add(peer.PeerCache.Account, peer);
            if (result)
            {
                peer.PeerCache.IsLogged = true;
                _Log.Info("----------------------------  AscensionServer.Cache.Login() : Server management logged peer success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                ReplaceLogin(peer);
                loggedPeerCache.Set(peer.PeerCache.Account, peer);
                _Log.Info("----------------------------  AscensionServer.Cache.Login() :  can't add into logged Dict------------------------------------" + loginPeerDict.ContainsKey(peer.PeerCache.Account));
            }
        }
        public void Logoff(AscensionPeer peer)
        {
            if (!peer.PeerCache.IsLogged)
                return;
            var result = loggedPeerCache.Remove(peer.PeerCache.Account);
            if (result)
            {
                peer.PeerCache.IsLogged = false;
                _Log.Info("---------------------------- AscensionServer.Cache.Logoff() :remove peer logoff success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                _Log.Info("---------------------------- AscensionServer.Cache.Logoff() : can't  remove from logged Dict  : " + peer.ToString() + "------------------------------------");
            }
        }
    }
}
