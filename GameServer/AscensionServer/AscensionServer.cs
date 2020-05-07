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

namespace AscensionServer
{
   public  class AscensionServer:ApplicationBase
    {
        public static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static AscensionServer ServerInstance
        {
            get;
            private set;
        }

        Dictionary<OperationCode, BaseHandler> handlerDict = new Dictionary<OperationCode, BaseHandler>();
        public Dictionary<OperationCode, BaseHandler> HandlerDict { get { return handlerDict; } }


        public List<MyClientPeer> peerList = new List<MyClientPeer>();  //通过这个集合可以访问到所有的客户

        ///当一个客户端请求连接的时候，服务器就会调用这个方法,我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            log.Info("一个客户端链接进来了！");

            var peer = new MyClientPeer(initRequest);
            peerList.Add(peer);
            return peer;
            //return new MyClientPeer(initRequest);
        }
        protected override void Setup()
        {
            ServerInstance = this;
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");//配置log的输出位置
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);//让log4net读取配置文件
                log.Info("进行初始化");
            }
            InitHandler();
            //SyncPositionThread.Run();
        }

        protected override void TearDown()
        {
            //SyncPositionThread.Stop();
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

        }
    }
}
