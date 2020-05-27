/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : Handler基类
*/
using Photon.SocketServer;
using AscensionProtocol;
namespace AscensionServer
{
    public abstract class BaseHandler
    {

        protected OperationCode opCode;
        public OperationCode OpCode { get { return OpCode; }protected set { opCode = value; } }
        protected EventCode evCode;
        public EventCode EvCode { get { return evCode; }protected set { evCode = value; } }

        //处理请求的方法
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,AscensionPeer peer);

    }
}
