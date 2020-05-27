using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer.Handler
{
    public class SyncRoleAssetsHandler : BaseHandler
    {
        public SyncRoleAssetsHandler()
        {
            opCode = OperationCode.SyncRoleAssets;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
        }
    }
}
