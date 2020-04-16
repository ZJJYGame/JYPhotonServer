using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
namespace AscensionServer
{
    public class ClientPeer : Photon.SocketServer.ClientPeer
    {
        public ClientPeer(InitRequest initRequest) : base(initRequest)
        {
        }
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
        }
    }
}
