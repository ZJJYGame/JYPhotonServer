using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public class UpdateRoleSubHandler : SyncRoleSubHandler
    {
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
        }
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
    }
}
