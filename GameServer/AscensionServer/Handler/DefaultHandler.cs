/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 默认处理者
*/
using Photon.SocketServer;

namespace AscensionServer
{
    public class DefaultHandler : BaseHandler
    {
        public DefaultHandler()
        {
            opCode = AscensionProtocol.OperationCode.Default;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
        }
    }
}
