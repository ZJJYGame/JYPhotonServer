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
    public class AscensionServer : ApplicationBase
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private SyncPositionThread syncPositionThread = new SyncPositionThread();
        new public static AscensionServer Instance { get; private set; }
        Dictionary<OperationCode, BaseHandler> handlerDict = new Dictionary<OperationCode, BaseHandler>();
        public Dictionary<OperationCode, BaseHandler> HandlerDict { get { return handlerDict; } }


        /// <summary>
        /// 已经连接但是未登录的客户端对象容器
        /// </summary>
         HashSet<AscensionPeer> connectedPeerHashSet = new HashSet<AscensionPeer>();
        public HashSet<AscensionPeer> ConnectedPeerHashSet { get { return connectedPeerHashSet; }set { connectedPeerHashSet = value; } }
        /// <summary>
        /// 连接且登录的客户端对象容器， <Key,Value>---<场景名，客户端容器>
        /// </summary>
        SortedList<string, ClientPeerContainer> peerContainerDict = new SortedList<string, ClientPeerContainer>();
        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="peer"></param>
        public void Connect(AscensionPeer peer)
        {
            connectedPeerHashSet.Add(peer);
        }
        /// <summary>
        /// 登录，并在服务器中注册
        /// </summary>
        /// <param name="peer"></param>
        public void RegisterPeer(AscensionPeer peer)
        {
            if (connectedPeerHashSet.Contains(peer))
            {
                var container = peerContainerDict[peer.OnlineStateDTO.CurrentScene];
                container.Add(peer);
            }
        }
        public void UpdateContainer(AscensionPeer peer)
        {
            if (connectedPeerHashSet.Contains(peer))
            {
                var previousContainer = peerContainerDict[peer.OnlineStateDTO.PreviousScene];
                previousContainer.Remove(peer);
                var currentContainer = peerContainerDict[peer.OnlineStateDTO.CurrentScene];
                currentContainer.Add(peer);
            }
        }
        /// <summary>
        /// 注销在服务器的登录
        /// </summary>
        /// <param name="peer"></param>
        public void DeregisterPeer(AscensionPeer peer)
        {
            if (connectedPeerHashSet.Contains(peer))
            {
                var container = peerContainerDict[peer.OnlineStateDTO.CurrentScene];
                container.Remove (peer);
                connectedPeerHashSet.Remove(peer);
            }
        }
        /// <summary>
        /// 仅仅从注册在场景的容器中取出 
        /// </summary>
        public void DeregisterPeerInScene(AscensionPeer peer)
        {
            if (connectedPeerHashSet.Contains(peer))
            {
                var container = peerContainerDict[peer.OnlineStateDTO.CurrentScene];
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
            log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~  PeerIP  " + peer.RemoteIP + "  Connected  ~~~~~~~~~~~~~~~~~~~~~~~~");
            PeerList.Add(peer);
            //Connect(peer);
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
            CreateHandler createHandle = new CreateHandler();
            handlerDict.Add(createHandle.opCode, createHandle);
            SelectRoleHandler selectRoleHandler = new SelectRoleHandler();
            handlerDict.Add(selectRoleHandler.opCode, selectRoleHandler);
            LogOutHandler logOutHandler = new LogOutHandler();
            handlerDict.Add(logOutHandler.opCode, logOutHandler);
            SyncRoleStatusHandler syncRoleStatusHandler = new SyncRoleStatusHandler();
            handlerDict.Add(syncRoleStatusHandler.opCode, syncRoleStatusHandler);
        }
    }
}
