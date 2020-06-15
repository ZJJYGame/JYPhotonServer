/*
*Author : Don
*Since 	:2020-04-18
*Description  : 服务器应用
*/
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
using Cosmos;
namespace AscensionServer
{
    public partial class AscensionServer : ApplicationBase
    {
        #region Properties
        public static IFiber _Fiber { get; private set; }
        public static readonly ILogger _Log = LogManager.GetCurrentClassLogger();
        private SyncPositionThread syncPositionThread = new SyncPositionThread();
        new public static AscensionServer Instance { get; private set; }
        Dictionary<OperationCode, Handler> handlerDict = new Dictionary<OperationCode, Handler>();
        public Dictionary<OperationCode, Handler> HandlerDict { get { return handlerDict; } }

        Dictionary<string, AscensionPeer> loginPeerDict = new Dictionary<string, AscensionPeer>();
        public Dictionary<string, AscensionPeer> LoginPeerDict { get { return loginPeerDict; } }

        HashSet<AscensionPeer> loggedPeer = new HashSet<AscensionPeer>();
        public HashSet<AscensionPeer> LoggedPeer { get { return loggedPeer; } }
        #endregion
        /// <summary>
        /// 当一个客户端请求连接的时候，服务器就会调用这个方法
        /// 我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new AscensionPeer(initRequest);
            _Log.Info("***********************  Client connected   ***********************");
            connectedPeerHashSet.Add(peer);
            //TODO  连接后的peer 添加到hashset里
            return peer;
        }
        protected override void Setup()
        {
            Instance = this;
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");//配置log的输出位置
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);//让log4net读取配置文件
                _Log.Info("进行初始化");
            }
            _Fiber = new PoolFiber();
            _Fiber.Start();
            InitHandler();
            Utility.Json.SetJsonWarpper(new NewtonjsonWrapper());
            //syncPositionThread.Run();
            ThreadEvent.AddSyncEvent(new SyncRoleTransformEvent());
            ThreadEvent.ExecuteEvent();
        }
        //TODO 服务器心跳检测
        protected override void TearDown()
        {
            //syncPositionThread.Stop();
            _Log.Info("***********************  Server Shotdown !!! ***********************");
            handlerDict.Clear();
        }
        void InitHandler()
        {
            var handlerType = typeof(Handler);
            Type[] types = Assembly.GetAssembly(handlerType).GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (handlerType.IsAssignableFrom(types[i]))
                {
                    if (types[i].IsClass && !types[i].IsAbstract)
                    {
                        var handler = Utility.Assembly.GetTypeInstance(types[i]) as Handler;
                        handler.OnInitialization();
                        _Log.Info("Handler start initialization : \n >>>>>" + handler.GetType().FullName + "<<<<<\n Initialization Done !");
                    }
                }
            }
        }
        public void RegisterHandler(Handler handler)
        {
            handlerDict.Add(handler.OpCode, handler);
        }
        public void DeregisterHandler(Handler handler)
        {
            handlerDict.Remove(handler.OpCode);
        }
        public bool IsLogin(AscensionPeer peer)
        {
            return loginPeerDict.ContainsKey(peer.PeerCache.Account);
        }
        Dictionary<string, AscensionPeer> onlinePeerDict = new Dictionary<string, AscensionPeer>();
        public void Online(AscensionPeer peer, int roleid)
        {

            var result = loggedPeerCache.Set(peer.PeerCache.Account, peer);
            if (result)
            {
                peer.PeerCache.RoleID = roleid;
                peer.PeerCache.IsLogged = true;
            }
            else
                _Log.Info("----------------------------  can't set into  logged Dict : " + peer.PeerCache.Account.ToString() + "------------------------------------");
            //try
            //{
            //    onlinePeerDict.Add(peer.PeerCache.Account, peer);
            //    RoleDTO roleDTO = new RoleDTO() { RoleID = roleid };
            //    PeerCache onlineStatusDTO = new PeerCache() { RoleID = roleDTO.RoleID,IsLogged=true };
            //    onlinePeerDict[peer.PeerCache.Account].PeerCache = onlineStatusDTO;
            //}
            //catch (Exception)
            //{
            //    _Log.Info("----------------------------  can't add into onlinePeerDict" + peer.PeerCache.Account.ToString() + "------------------------------------");
            //}
        }
        public void Offline(AscensionPeer peer)
        {
            var result = loggedPeerCache.Remove(peer.PeerCache.Account);
            if (!result)
                _Log.Info("----------------------------  can't  remove from logged Dict : " + peer.PeerCache.Account.ToString() + "------------------------------------");
            //try
            //{
            //    onlinePeerDict.Remove(peer.PeerCache.Account);
            //}
            //catch (Exception)
            //{
            //    _Log.Info("----------------------------  can't add into offlinePeerDict" + peer.PeerCache.Account.ToString() + "------------------------------------");
            //}
        }
        public int HasOnlineID(AscensionPeer peer)
        {
            if (onlinePeerDict.ContainsKey(peer.PeerCache.Account))
            {
                return onlinePeerDict[peer.PeerCache.Account].PeerCache.RoleID;
            }
            else
                return 0;
        }
        //处理登录冲突部分代码
        void ReplaceLogin(AscensionPeer peer)
        {
            EventData ed = new EventData((byte)EventCode.ReplacePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.ForcedOffline, 0);
            ed.Parameters = data;
            //loginPeerDict[peer.PeerCache.Account].SendEvent(ed, new SendParameters());
            loggedPeerCache[peer.PeerCache.Account].SendEvent(ed, new SendParameters());
            _Log.Info("登录冲突检测》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");
        }
    }
}
