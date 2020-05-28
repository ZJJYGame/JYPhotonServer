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
    public abstract class BaseHandler
    {

        protected OperationCode opCode;
        public OperationCode OpCode { get { return opCode; }protected set { opCode = value; } }
        protected EventCode evCode;
        public EventCode EvCode { get { return evCode; }protected set { evCode = value; } }
        protected OperationResponse opResponse;
        public OperationResponse OpResponse { get { if (opResponse == null) opResponse = new OperationResponse();
                return opResponse; }protected set { opResponse = value; } }
        public Dictionary<byte, object> ResponseData { get { if (responseData == null) responseData = new Dictionary<byte, object>();
                return responseData;} protected set { responseData = value; }}
        protected Dictionary<byte, object> responseData;
        //处理请求的方法
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,AscensionPeer peer);

    }
}
