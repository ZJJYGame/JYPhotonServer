using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class PeerInitHandler : BaseHandler
    {
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

        }
    }
}
