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
    public class JYClientPeer : Photon.SocketServer.ClientPeer
    {
        public float x, y, z;
        public string username;
        public string uuid;
        public JYClientPeer(InitRequest initRequest) : base(initRequest)
        {
        }
        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            ////告诉其他客户端有客户端断开链接    
            foreach (JYClientPeer temPeer in AscensionServer.ServerInstance.peerList)
            {
                if (!string.IsNullOrEmpty(temPeer.username)&&temPeer.username!=username)
                {
                    EventData ed = new EventData((byte)EventCode.DeletePlayer);
                    Dictionary<byte, object> data2 = new Dictionary<byte, object>();
                    data2.Add((byte)ParameterCode.UserCode.Username,username);
                    ed.Parameters = data2;
                    temPeer.SendEvent(ed, new SendParameters()); //发送事件sendParameters                
                }
            }
              AscensionServer.ServerInstance.peerList.Remove(this);
        }


        //处理客户端的请求
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            //OperationRequest封装了请求的信息
            //SendParameters 参数，传递的数据
            AscensionServer.log.Info("收到一个客户端的请求");
            
            //通过客户端的OperationCode从HandlerDict里面获取到了需要的Hander
            BaseHandler handler = Utility.GetValue<OperationCode, BaseHandler>(AscensionServer.ServerInstance.HandlerDict, (OperationCode)operationRequest.OperationCode);

            //如果找到了需要的hander就调用我们hander里面处理请求的方法
            if (handler != null)
            {
                //Dictionary<byte, object> data = operationRequest.Parameters;
                //object inVaule;
                //data.TryGetValue(0, out inVaule);
                //object inVaule2;
                //data.TryGetValue(1, out inVaule2);
                //AscensionServer.log.Info("从客户端得到数据 ： " + inVaule.ToString() + "     数据拼接    " + inVaule2.ToString());
                handler.OnOperationRequest(operationRequest, sendParameters, this);

            }
            else//否则我们就使用默认的hander
            {
                BaseHandler defaultHandler = Utility.GetValue<OperationCode, BaseHandler>(AscensionServer.ServerInstance.HandlerDict, OperationCode.Default);
                defaultHandler.OnOperationRequest(operationRequest, sendParameters, this);
            }
        }
    }
 }
