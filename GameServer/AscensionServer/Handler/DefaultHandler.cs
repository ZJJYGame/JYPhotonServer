using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace AscensionServer
{
    public class DefaultHandler : BaseHandler
    {
        public DefaultHandler()
        {
            opCode = AscensionProtocol.OperationCode.Default;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
        }
    }
}
