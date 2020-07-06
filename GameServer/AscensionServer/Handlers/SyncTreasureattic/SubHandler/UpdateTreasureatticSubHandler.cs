using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdateTreasureatticSubHandler : SyncTreasureatticSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.Treasureattic));
            var treasureatticObj = Utility.Json.ToObject<Treasureattic>(treasureatticJson);
            NHCriteria nHCriteriaTreasureattic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);
            var treasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
        }
    }
}
