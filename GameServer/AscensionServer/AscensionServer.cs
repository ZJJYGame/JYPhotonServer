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
namespace AscensionServer
{
   public  class AscensionServer:ApplicationBase
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private SyncPositionThread syncPositionThread = new SyncPositionThread();
       new public static AscensionServer Instance
        {
            get;
            private set;
        }
        Dictionary<OperationCode, BaseHandler> handlerDict = new Dictionary<OperationCode, BaseHandler>();
        public Dictionary<OperationCode, BaseHandler> HandlerDict { get { return handlerDict; } }

        SortedList<int, AscensionPeer> jyClientPeerDict = new SortedList<int, AscensionPeer>();
        public  SortedList<int, AscensionPeer> JYClientPeerDict { get { return jyClientPeerDict; } set { jyClientPeerDict = value; } }

        List<AscensionPeer> peerList = new List<AscensionPeer>();  //通过这个集合可以访问到所有的客户
        public List<AscensionPeer> PeerList { get { return peerList; } }

        public int ClientCount { get { return PeerList.Count; } }
        int clientCount = 0;

        ///当一个客户端请求连接的时候，服务器就会调用这个方法,我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            log.Info("***********************  Client connected !!! ***********************");
            var peer = new AscensionPeer(initRequest);
            log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~  PeerIP  "+peer.RemoteIP+"  Connected  ~~~~~~~~~~~~~~~~~~~~~~~~");
            PeerList.Add(peer);
            UpdatePeer(ref peer);
            return peer;
        }
        void UpdatePeer(ref AscensionPeer peer)
        {
            peer.PeerID = clientCount;
            jyClientPeerDict.Add(peer.PeerID, peer);
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
        }
    }
}
