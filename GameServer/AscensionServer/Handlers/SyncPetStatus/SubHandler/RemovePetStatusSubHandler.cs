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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
        var dict = ParseSubDict(operationRequest);
            string petstatus = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.PetStatus));
        }
    }
}
