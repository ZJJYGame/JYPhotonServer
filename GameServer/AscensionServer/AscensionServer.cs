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


        public List<ClientPeer> peerList = new List<ClientPeer>();

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            log.Info("Client connected");
            /*var peer = new MyClientPeer(initRequest);
            peerList.Add(peer);
            return peer;*/
            return new MyClientPeer(initRequest);
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
                log.Info("log4net.config file exist");
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

        }
    }
}
