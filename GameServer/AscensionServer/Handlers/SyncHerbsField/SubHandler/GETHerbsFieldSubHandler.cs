using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer.Handlers
{
    public class GetHerbsFieldSubHandler : SyncHerbsFieldSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            var hfObj = Utility.Json.ToObject<HerbsField>(treasureatticJson);
        }
    }
}
