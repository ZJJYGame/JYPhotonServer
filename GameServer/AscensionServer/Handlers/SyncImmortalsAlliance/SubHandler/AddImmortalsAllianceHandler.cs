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
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
    public class AddImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatusDTO>
                (immortalsAllianceJson);

            NHCriteria nHCriteriaAllianceName = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", alliancestatusObj.AllianceName);
            var alliance = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ImmortalsAlliance>(nHCriteriaAllianceName);
            if (alliance==null)
            {
                List<int> gangslist = new List<int>();
                NHCriteria nHCriteriaAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);
                var allianceslIstObj= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaAlliance);
                gangslist = Utility.Json.ToObject<List<int>>(allianceslIstObj.AllianceList);


            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
