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
using AscensionServer.Model;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;

namespace AscensionServer
{
    public partial class AscensionServer:ApplicationBase
    {
        #region Properties
        /// <summary>
        /// 已经连接但是未登录的客户端对象容器
        /// </summary>
        HashSet<AscensionPeer> connectedPeerHashSet = new HashSet<AscensionPeer>();
        public HashSet<AscensionPeer> ConnectedPeerHashSet { get { return connectedPeerHashSet; } set { connectedPeerHashSet = value; } }

        Cache<AscensionPeer> loggedPeerCache = new Cache<AscensionPeer>();
        public Cache<AscensionPeer> LoggedPeerCache { get { return loggedPeerCache; } }
        Cache<AscensionPeer> addventureScenePeerCache = new Cache<AscensionPeer>();
        public Cache<AscensionPeer>   AdventureScenePeerCache{ get { return addventureScenePeerCache; } }

        #endregion
        #region Methods
        public void RemoveFromLoggedUserCache(AscensionPeer peer)
        {
            if (!peer.PeerCache.IsLogged)
                return;
            var result = loggedPeerCache.Remove(peer.PeerCache.Account);
            if (result)
            {
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() :remove peer logoff success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() : can't  remove from logged Dict  : " + peer.ToString() + "------------------------------------");
            }
        }
        public int HasOnlineID(AscensionPeer peer)
        {
            if (loggedPeerCache.IsExists(peer.PeerCache.Account))
            {
                return loggedPeerCache[peer.PeerCache.Account].PeerCache.RoleID;
            }
            else
                return -1;
        }
        //处理登录冲突部分代码
        void ReplaceLogin(AscensionPeer peer)
        {
            peer.PeerCache.IsLogged = false;
            EventData ed = new EventData((byte)EventCode.ReplacePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.ForcedOffline, 0);
            ed.Parameters = data;
            //loginPeerDict[peer.PeerCache.Account].SendEvent(ed, new SendParameters());
            loggedPeerCache[peer.PeerCache.Account].SendEvent(ed, new SendParameters());
            Utility.Debug.LogInfo("登录冲突检测》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");
        }
        public void Online(AscensionPeer peer, Role role)
        {
            var result = loggedPeerCache.Set(peer.PeerCache.Account, peer);
            if (result)
            {
                peer.PeerCache.RoleID = role.RoleID;
                peer.PeerCache.Role = role;
                peer.PeerCache.IsLogged = true;
                Utility.Debug.LogInfo("----------------------------  AscensionServer.Offline() : online success : " + peer.PeerCache.Account.ToString() + "------------------------------------");
            }
            else
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Online()  :  can't set into  logged Dict : " + peer.PeerCache.Account.ToString() + "------------------------------------");
        }
        public void Offline(AscensionPeer peer)
        {
            if (!peer.PeerCache.IsLogged)
                return;
            var result = loggedPeerCache.Remove(peer.PeerCache.Account);
            if (!result)
                Utility.Debug.LogInfo("----------------------------  AscensionServer.Offline() :can't  remove from logged Dict : " + peer.PeerCache.Account.ToString() + "------------------------------------");
            else
                Utility.Debug.LogInfo("----------------------------  AscensionServer.Offline() : offline success : " + peer.PeerCache.Account.ToString() + "------------------------------------");
        }

        /// <summary>
        /// 进入与离开探索界面方法只是缓兵之计，需要合理设计
        /// </summary>
        /// <param name="peer"></param>
        // TODO 进入探索界面容器需要优化，未对结构进行设计
        public void EnterAdventureScene(AscensionPeer peer,Action callBack=null)
        {
            var result = addventureScenePeerCache.Add(peer.PeerCache.Account, peer);
            if (result)
            {
                callBack?.Invoke();
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() :remove peer addventureScenePeerCache  success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() : can't  remove from laddventureScenePeerCache  : " + peer.ToString() + "------------------------------------");
            }
        }
        /// <summary>
        /// 离开探索界面
        /// </summary>
        /// <param name="peer">peer对象</param>
        /// <param name="callBack">离开成功后执行的回调</param>
        public void ExitAdventureScene(AscensionPeer peer,Action callBack=null)
        {
            var result = addventureScenePeerCache.Remove(peer.PeerCache.Account);
            if (result)
            {
                callBack?.Invoke();
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() :remove peer addventureScenePeerCache success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() : can't  remove from laddventureScenePeerCache : " + peer.ToString() + "------------------------------------");
            }
        }
        public bool IsEnterAdventureScene(AscensionPeer peer)
        {
            var result = addventureScenePeerCache.IsExists(peer.PeerCache.Account);
            return result;
        }
        #endregion
    }
}
