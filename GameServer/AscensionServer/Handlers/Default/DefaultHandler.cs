/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 默认处理者
*/
using Photon.SocketServer;

namespace AscensionServer
{
    public class DefaultHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.Default;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

        }
    }
}
