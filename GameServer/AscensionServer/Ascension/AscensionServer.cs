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
using AscensionServer.Model;
using AscensionData;
using AscensionRegion;
using System.Threading;
namespace AscensionServer
{
    public partial class AscensionServer : ApplicationBase
    {
        #region Properties
        new public static AscensionServer Instance { get; private set; }
        public Dictionary<OperationCode, Handler> HandlerDict { get; private set; }
        #endregion
        /// <summary>
        /// 当一个客户端请求连接的时候，服务器就会调用这个方法
        /// 我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        /// </summary>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new AscensionPeer(initRequest);
            Utility.Debug.LogInfo("Peer connect");
            ConnectedPeerHashSet.Add(peer);
            return peer;
        }
        protected override void Setup()
        {
            Utility.Debug.SetHelper(new Log4NetDebugHelper());
            HandlerDict = new Dictionary<OperationCode, Handler>();
            Instance = this;
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");//配置log的输出位置
            FileInfo configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);//让log4net读取配置文件
                Utility.Debug.LogInfo("Server Start Running");
            }
            InitHandler();
            Utility.Json.SetHelper(new NewtonjsonHelper());
            var syncRoleTransEvent = new SyncRoleTransformEvent();
            syncRoleTransEvent.OnInitialization();
            ThreadPool.QueueUserWorkItem(syncRoleTransEvent.Handler);
            var syncRefreshResourcesEvent = new SyncRefreshResourcesEvent();
            syncRefreshResourcesEvent.OnInitialization();
            ThreadPool.QueueUserWorkItem(syncRefreshResourcesEvent.Handler);
            GameManager.External.GetModule<ResourceManager>().ResourcesLoad();
            RedisDotNet.RedisManager.Instance.OnInitialization();
        }
        protected override void TearDown()
        {
            Utility.Debug.LogInfo("Server Shutdown");
            HandlerDict.Clear();
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
                        HandlerDict.Add(handler.OpCode, handler);
                    }
                }
            }
        }
    }
}
