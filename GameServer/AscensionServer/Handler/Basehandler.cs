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

        public OperationCode opCode;

        public EventCode evCode;
        //处理请求的方法
        public abstract void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters,JYClientPeer peer);

    }
}
