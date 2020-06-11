﻿/*
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
namespace AscensionServer
{
    public class AscensionServer : ApplicationBase
    {
        public static IFiber _Fiber { get; private set; }
        public static readonly ILogger _Log = LogManager.GetCurrentClassLogger();
        private SyncPositionThread syncPositionThread = new SyncPositionThread();
        new public static AscensionServer Instance { get; private set; }
        Dictionary<OperationCode, Handler> handlerDict = new Dictionary<OperationCode, Handler>();
        public Dictionary<OperationCode, Handler> HandlerDict { get { return handlerDict; } }

        /// <summary>
        /// 已经连接但是未登录的客户端对象容器
        /// </summary>
         HashSet<AscensionPeer> connectedPeerHashSet = new HashSet<AscensionPeer>();
        public HashSet<AscensionPeer> ConnectedPeerHashSet { get { return connectedPeerHashSet; }set { connectedPeerHashSet = value; } }
        List<AscensionPeer> peerList = new List<AscensionPeer>();  //通过这个集合可以访问到所有的
        public List<AscensionPeer> PeerList { get { return peerList; } }
        /// <summary>
        /// 当一个客户端请求连接的时候，服务器就会调用这个方法
        /// 我们使用peerbase，表示和一个客户端的连接，然后photon就会把链接管理起来
        /// </summary>
        /// <param name="initRequest"></param>
        /// <returns></returns>
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            var peer = new AscensionPeer(initRequest);
            _Fiber = new PoolFiber();
            _Fiber.Start();
            _Log.Info("***********************  \nClient connected !!! \n ***********************");
            PeerList.Add(peer);
            //TODO  连接后的peer 添加到hashset里
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
                _Log.Info("进行初始化");
            }
            InitHandler();
            //syncPositionThread.Run();
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
        public  void  DeregisterHandler(Handler handler)
        {
            handlerDict.Remove(handler.OpCode);
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
                ReplaceLogin(peer);
                loginPeerDict.Remove(peer.User.Account);
               
                loginPeerDict.Add(peer.User.Account, peer);
                _Log.Info("----------------------------  can't add into loginDict"+peer.User.Account.ToString()+"------------------------------------");
               
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
                _Log.Info("----------------------------  can't  remove from loginDict" + peer.ToString() + "------------------------------------");
            }
        }
        public bool IsLogin(AscensionPeer peer)
        {
            return loginPeerDict.ContainsKey(peer.User.Account);
        }
        Dictionary<string, AscensionPeer> onlinePeerDict = new Dictionary<string, AscensionPeer>();
        public void online(AscensionPeer peer, int roleid)
        {
            try
            {
                onlinePeerDict.Add(peer.User.Account, peer);
                RoleDTO roleDTO = new RoleDTO() { RoleID = roleid };
                OnlineStatusDTO onlineStatusDTO = new OnlineStatusDTO() { RoleID = roleDTO.RoleID };
                onlinePeerDict[peer.User.Account].OnlineStateDTO = onlineStatusDTO;
            }
            catch (Exception)
            {
                _Log.Info("----------------------------  can't add into onlinePeerDict" + peer.User.Account.ToString() + "------------------------------------");
            }
        }
        public void offline(AscensionPeer peer)
        {
            try
            {
                onlinePeerDict.Remove(peer.User.Account);
            }
            catch (Exception)
            {
                _Log.Info("----------------------------  can't add into offlinePeerDict" + peer.User.Account.ToString() + "------------------------------------");
            }
        }
        public int HasOnlineID(AscensionPeer peer)
        {
            if (onlinePeerDict.ContainsKey(peer.User.Account))
            {
                return onlinePeerDict[peer.User.Account].OnlineStateDTO.RoleID;
            }
            else
                return 0;
        }
        //处理登录冲突部分代码
        void ReplaceLogin(AscensionPeer peer)
        {
            EventData ed = new EventData((byte)EventCode.ReplacePlayer);
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.ForcedOffline, (byte)0);
            ed.Parameters = data;
            peer.SendEvent(ed,new SendParameters());
            _Log.Info("登录冲突检测》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》》");
        }
    }
}
