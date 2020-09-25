/*
*Author : Don
*Since 	:2020-04-18
*Description  : 服务器入口类
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
using Cosmos;
using System.Threading;
namespace AscensionServer
{
    public partial class AscensionServer : ApplicationBase
    {
        uint peerSession = 1000;
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            ++peerSession;
            var peer = new AscensionPeer(initRequest, peerSession);
            var peerEntity = PeerEntity.Create(peer);
            GameManager.CustomeModule<PeerManager>().TryAdd(peerEntity);
            return peer;
        }
        protected override void Setup()
        {
            Utility.Debug.SetHelper(new Log4NetDebugHelper());
            Utility.Json.SetHelper(new NewtonjsonHelper());
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");//配置log的输出位置
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);//让log4net读取配置文件
                Utility.Debug.LogInfo("Server Start Running");
            }
            NHibernateQuerier.Init();
            GameManager.InitCustomeModule(this.GetType().Assembly);
            RedisDotNet.RedisManager.Instance.OnInitialization();
        }
        protected override void TearDown()
        {
            Utility.Debug.LogInfo("Server Shutdown");
            GameManager.Instance.Dispose();
        }
    }
}
