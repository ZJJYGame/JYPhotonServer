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
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Handler;
using AscensionServer.Threads;
using ExitGames.Concurrency.Fibers;
namespace AscensionServer
{
   public  class AscensionServer:ApplicationBase
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private SyncPositionThread syncPositionThread = new SyncPositionThread();
       new public static AscensionServer Instance {get;private set; }
        Dictionary<OperationCode, BaseHandler> handlerDict = new Dictionary<OperationCode, BaseHandler>();
        public Dictionary<OperationCode, BaseHandler> HandlerDict { get { return handlerDict; } }

        SortedList<string, AscensionPeer> jyClientPeerDict = new SortedList<string, AscensionPeer>();
        public  SortedList<string, AscensionPeer> JYClientPeerDict { get { return jyClientPeerDict; } set { jyClientPeerDict = value; } }

        /// <summary>
        /// 已经连接但是未登录的客户端对象容器
        /// </summary>
        HashSet<AscensionPeer> connectedPeer = new HashSet<AscensionPeer>();
        /// <summary>
        /// 连接且登录的客户端对象容器
        /// </summary>
        SortedList<string, AscensionPeer> loginedPeer = new SortedList<string, AscensionPeer>();

        SortedList<string,ClientPeerContainer>containerDict=new SortedList<string, ClientPeerContainer>();
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="peer"></param>
        public void Connect(AscensionPeer peer)
        {
            connectedPeer.Add(peer);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="peer"></param>
        public void Login(AscensionPeer peer)
        {
            if (connectedPeer.Contains(peer))
            {
                connectedPeer.Remove(peer);
                loginedPeer.Add(peer.User.Account, peer);
            }
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="peer"></param>
        public void Disconnect(AscensionPeer peer)
        {
            if (peer.IsLogined)
            {
                try
                {
                    loginedPeer.Remove(peer.User.Account);
                }
                catch (Exception)
                {
                    log.Info("Logined account not exist" + peer.User.Account);
                    throw new Exception("Logined account not exist"+peer.User.Account);
                }
            }
        }

        List<AscensionPeer> peerList = new List<AscensionPeer>();  //通过这个集合可以访问到所有的
        public List<AscensionPeer> PeerList { get { return peerList; } }

        public int ClientCount { get { return PeerList.Count; } }
        int clientCount = 0;
        /// <summary>
        /// 当一个客户端请求连接的时候，服务器就会调用这个方法
        /// 我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new AscensionPeer(initRequest);
            log.Info("***********************  Client connected !!! ***********************");
            log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~  PeerIP  "+peer.RemoteIP+"  Connected  ~~~~~~~~~~~~~~~~~~~~~~~~");
            PeerList.Add(peer);
            //UpdatePeer(ref peer);
            return peer;
        }
        void UpdatePeer(ref AscensionPeer peer)
        {
            peer.PeerID = clientCount;
            jyClientPeerDict.Add(peer.RemoteIP, peer);
            clientCount++;
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
                log.Info("进行初始化");
            }
            InitHandler();
            syncPositionThread.Run();
        }
        protected override void TearDown()
        {
            syncPositionThread.Stop();
            log.Info("***********************  Server Shotdown !!! ***********************");
        }
        void InitHandler()
        {
            LoginHandler loginHandler = new LoginHandler();
            handlerDict.Add(loginHandler.opCode, loginHandler);
            DefaultHandler defaultHandler = new DefaultHandler();
            handlerDict.Add(defaultHandler.opCode, defaultHandler);
            RegisterHandler registerHandler = new RegisterHandler();
            handlerDict.Add(registerHandler.opCode, registerHandler);
            SyncPositionHandler syncPositionHandler = new SyncPositionHandler();
            handlerDict.Add(syncPositionHandler.opCode, syncPositionHandler);
            SyncPlayerHandler syncPlayerHandler = new SyncPlayerHandler();
            handlerDict.Add(syncPlayerHandler.opCode, syncPlayerHandler);
            CreateHandler  createHandle = new CreateHandler();
            handlerDict.Add(createHandle.opCode, createHandle);
            SelectRoleHandler selectRoleHandler = new SelectRoleHandler();
            handlerDict.Add(selectRoleHandler.opCode, selectRoleHandler);
            LogoffHandler logoffHandler = new LogoffHandler();
            handlerDict.Add(logoffHandler.opCode, logoffHandler);
            SyncRoleStatusHandler syncRoleStatusHandler = new SyncRoleStatusHandler();
            handlerDict.Add(syncRoleStatusHandler.opCode, syncRoleStatusHandler);
        }
    }
}
