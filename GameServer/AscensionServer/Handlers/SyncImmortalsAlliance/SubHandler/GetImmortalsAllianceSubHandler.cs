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

namespace AscensionServer
{
    public class GetImmortalsAllianceSubHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var immortalsAllianceObj = Utility.Json.ToObject<ImmortalsAllianceDTO>(immortalsAllianceJson);
            var name = RedisData.Initialize.InsertName("(ALLIANCE_LIST", immortalsAllianceObj.ID);
            var content = RedisData.Initialize.GetData(name);
            if (string.IsNullOrEmpty(content))
            {
                NHCriteria nHCriteriaimmortalsAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", immortalsAllianceObj.ID);
                var immortalsAllianceTemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ImmortalsAlliance>();


            }
        }
    }
}
