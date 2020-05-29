/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : Handler基类
*/
using Photon.SocketServer;
using AscensionProtocol;
using System.Collections.Generic;
namespace AscensionServer
{
    public abstract class HandlerBase:IBehaviour
    {
        public OperationCode OpCode { get; protected set; }
        public EventCode EvCode { get; protected set; }
        protected OperationResponse opResponse ;
        public OperationResponse OpResponse { get { if (opResponse == null)
                    opResponse = new OperationResponse();
                return opResponse; }protected set { opResponse = value; } }
        public Dictionary<byte, object> ResponseData { get { if (responseData == null)
                    responseData = new Dictionary<byte, object>();
                return responseData;} protected set { responseData = value; }}
        protected Dictionary<byte, object> responseData;
        //处理请求的方法
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,AscensionPeer peer);
        public virtual void OnInitialization()
        {
            AscensionServer.Instance.RegisterHandler(this);
        }
        public virtual void OnTermination()
        {
            AscensionServer.Instance.DeregisterHandler(this);
        }
    }
}
