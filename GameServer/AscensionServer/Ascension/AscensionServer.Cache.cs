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
        /// <summary>
        /// 角色在主界面时候，相当于在游戏大厅
        /// </summary>
        // TODO 需要添加大厅功能
        Cache<AscensionPeer> lobby = new Cache<AscensionPeer>();
        public Cache<AscensionPeer> Lobby { get { return lobby; } }

        Cache<AscensionPeer> refreshpool = new Cache<AscensionPeer>();
        public Cache<AscensionPeer> RefreshPool { get { return refreshpool; } }
        #endregion

        #region Methods
        public void AddIntoLoggedUserCache(AscensionPeer peer)
        {
            var result = loggedPeerCache.Add(peer.PeerCache.Account, peer);
            if (result)
            {
                Utility.Debug.LogInfo("----------------------------  AscensionServer.Cache.Login() : Server management logged peer success : " + peer.ToString() + "------------------------------------");
            }
            else
            {
                if (loggedPeerCache.TryGetValue(peer.PeerCache.Account,out AscensionPeer oldpeer))
                {
                    ReplaceLogin(oldpeer);
                    loggedPeerCache.Remove(oldpeer.PeerCache.Account);
                }
                loggedPeerCache.Add(peer.PeerCache.Account, peer);
            }
        }
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
        #region 刷新机制
        public void RefreshData()
        {
            System.Timers.Timer t = new System.Timers.Timer(10000);//实例化Timer类，设置时间间隔
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            t.Elapsed += new System.Timers.ElapsedEventHandler(Method2);//到达时间的时候执行事件
        }
        void Method2(object source, System.Timers.ElapsedEventArgs e)
        {
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            if (intHour == 4&& intMinute ==0&& intSecond == 0)
            {
                RefreshPool.Clear();
            }
        }
        #endregion
    }
}
