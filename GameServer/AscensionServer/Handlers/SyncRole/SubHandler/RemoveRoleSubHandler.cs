using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using Ubiety.Dns.Core.Common;

namespace AscensionServer
{
    public class RemoveRoleSubHandler : SyncRoleSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            return operationResponse;
        }
    }
}
