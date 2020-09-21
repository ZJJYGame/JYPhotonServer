using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public class RemoveRoleSubHandler : SyncRoleSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
        }
    }
}
