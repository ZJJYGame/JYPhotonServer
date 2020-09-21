/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 默认处理者
*/
using NHibernate.Tuple.Component;
using Photon.SocketServer;

namespace AscensionServer
{
    public class DefaultHandler : Handler
    {
        public override byte OpCode { get {return (byte) AscensionProtocol.OperationCode.Default; } }

        protected override OperationResponse  OnOperationRequest(OperationRequest operationRequest)
        {
            return OpResponseData;
        }
    }
}
