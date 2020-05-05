using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using AscensionProtocol.Model.UserClient;

namespace AscensionServer
{
    //管理跟客户端的链接的
    public class MyClientPeer : Photon.SocketServer.ClientPeer
    {
        
        //public User User { get; set; }
        //public Role Role { get; set; }
        public float x, y, z;
        public string username;
        public string uuid;
        public MyClientPeer(InitRequest initRequest) : base(initRequest)
        {


        }
        //处理客户端断开连接的后续工作
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
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
