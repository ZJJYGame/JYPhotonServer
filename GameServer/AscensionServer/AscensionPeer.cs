/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 客户端类
*/
using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using System.Collections.Generic;

namespace AscensionServer
{
    //管理跟客户端的链接的
    public class AscensionPeer : ClientPeer
    {
        public float x, y, z;
        string account;
        public string Account { get { return account; } set { account = value; } }
        public string UUID { get; set; }
        int peerID = -1;
        public int PeerID { get { return peerID; }set { peerID = value; } }
        public AscensionPeer(InitRequest initRequest) : base(initRequest) { }
        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            ////告诉其他客户端有客户端断开链接    
            foreach (AscensionPeer temPeer in AscensionServer.Instance.PeerList)
            {
                if (!string.IsNullOrEmpty(temPeer.Account)&&temPeer.Account!=Account)
                {
                    EventData ed = new EventData((byte)EventCode.DeletePlayer);
                    Dictionary<byte, object> data = new Dictionary<byte, object>();
                    data.Add((byte)ParameterCode.UserCode.Username,Account);
                    ed.Parameters = data;
                    temPeer.SendEvent(ed, new SendParameters()); //发送事件sendParameters                
                }
            }
              AscensionServer.Instance.PeerList.Remove(this);
            AscensionServer.log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~ PeerIP  " + RemoteIP + "  Disconnected~~~~~~~~~~~~~~~~~~~~~~~~");
        }
        //处理客户端的请求
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //OperationRequest封装了请求的信息
            //SendParameters 参数，传递的数据
            AscensionServer.log.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~ PeerIP  " + RemoteIP + "  Request  ~~~~~~~~~~~~~~~~~~~~~~~~");

            //通过客户端的OperationCode从HandlerDict里面获取到了需要的Hander
            BaseHandler handler = Utility.GetValue(AscensionServer.Instance.HandlerDict, (OperationCode)operationRequest.OperationCode);
            //如果找到了需要的hander就调用我们hander里面处理请求的方法
            if (handler != null)
            {
                handler.OnOperationRequest(operationRequest, sendParameters, this);
            }
            else//否则我们就使用默认的hander
            {
                BaseHandler defaultHandler = Utility.GetValue<OperationCode, BaseHandler>(AscensionServer.Instance.HandlerDict, OperationCode.Default);
                defaultHandler.OnOperationRequest(operationRequest, sendParameters, this);
            }
        }
    }
 }
