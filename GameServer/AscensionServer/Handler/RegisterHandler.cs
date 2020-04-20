using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;

namespace AscensionServer
{
    public class RegisterHandler : BaseHandler
    {
        public RegisterHandler()
        {
            opCode = AscensionProtocol.OperationCode.Register;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, ClientPeer peer)
        {
        }
    }
}
