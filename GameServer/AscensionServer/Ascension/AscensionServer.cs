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
            //TODO  连接后的peer 添加到hashset里
            //ConnectedPeerHashSet.Add(peer);
            return peer;
        }
        /// <summary>
        /// 管理已登录的客户端
        /// </summary>
        Dictionary<string, AscensionPeer> ALLClientPeer = new Dictionary<string, AscensionPeer>();
        public Dictionary<string, AscensionPeer> AllClientPeer { get { return ALLClientPeer; }set { ALLClientPeer = value; } }





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
            //syncPositionThread.Run();
        }
        //TODO 服务器心跳检测
        protected override void TearDown()
        {
            //syncPositionThread.Stop();
            log.Info("***********************  Server Shotdown !!! ***********************");
        }
        void InitHandler()
        {
            LoginHandler loginHandler = new LoginHandler();
            handlerDict.Add(loginHandler.OpCode, loginHandler);
            DefaultHandler defaultHandler = new DefaultHandler();
            handlerDict.Add(defaultHandler.OpCode, defaultHandler);
            RegisterHandler registerHandler = new RegisterHandler();
            handlerDict.Add(registerHandler.OpCode, registerHandler);
            SyncPositionHandler syncPositionHandler = new SyncPositionHandler();
            handlerDict.Add(syncPositionHandler.OpCode, syncPositionHandler);
            SyncOtherRolesHandler syncPlayerHandler = new SyncOtherRolesHandler();
            handlerDict.Add(syncPlayerHandler.OpCode, syncPlayerHandler);
            CreateRoleHandler createHandle = new CreateRoleHandler();
            handlerDict.Add(createHandle.OpCode, createHandle);
            SelectRoleHandler selectRoleHandler = new SelectRoleHandler();
            handlerDict.Add(selectRoleHandler.OpCode, selectRoleHandler);
            LogoffHandler logoffHandler = new LogoffHandler();
            handlerDict.Add(logoffHandler.OpCode, logoffHandler);
            SyncRoleStatusHandler syncRoleStatusHandler = new SyncRoleStatusHandler();
            handlerDict.Add(syncRoleStatusHandler.OpCode, syncRoleStatusHandler);
            VerifyRoleStatusHandler verifyRoleStatusHandler = new VerifyRoleStatusHandler();
            handlerDict.Add(verifyRoleStatusHandler.OpCode, verifyRoleStatusHandler);
            //DistributeTaskHandler distributeTaskHandler = new DistributeTaskHandler();
            //handlerDict.Add(distributeTaskHandler.OpCode, distributeTaskHandler);
            HeartBeatHandler heartBeatHandler = new HeartBeatHandler();
            handlerDict.Add(heartBeatHandler.OpCode, heartBeatHandler);
            SyncRoleAssetsHandler syncRoleAssetsHandler = new SyncRoleAssetsHandler();
            handlerDict.Add(syncRoleAssetsHandler.OpCode, syncRoleAssetsHandler);
        }
        Dictionary<string, AscensionPeer> loginPeerDict = new Dictionary<string, AscensionPeer>();
        public void Login(AscensionPeer peer)
        {
            try
            {
                loginPeerDict.Add(peer.User.Account, peer);
            }
            catch (Exception)
            {
                log.Info("----------------------------  can't add into loginDict"+peer.User.Account.ToString()+"------------------------------------");
            }
        }
        public void Logoff(AscensionPeer peer)
        {
            try
            {
                loginPeerDict.Remove(peer.User.Account);
            }
            catch (Exception)
            {
                log.Info("----------------------------  can't  remove from loginDict" + peer.ToString() + "------------------------------------");
            }
        }
        public bool IsLogin(AscensionPeer peer)
        {
            return loginPeerDict.ContainsKey(peer.User.Account);
        }
    }
}
