using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer.Handlers.SyncPetStatus.SubHandler
{

    public class RemovePetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            return operationResponse;
        }
    }
}
