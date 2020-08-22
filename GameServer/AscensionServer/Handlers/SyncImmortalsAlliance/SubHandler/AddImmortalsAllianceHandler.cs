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
            string alliancestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatusDTO>
                (alliancestatusJson);
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var immortalsAllianceObj = Utility.Json.ToObject<RoleAllianceDTO> 
                (immortalsAllianceJson);


            NHCriteria nHCriteriaAllianceName = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);
            var alliance = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriaAllianceName);
            if (alliance==null)
            {
                List<int> gangslist = new List<int>();
                NHCriteria nHCriteriaAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);
                var allianceslIstObj= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaAlliance);
                gangslist = Utility.Json.ToObject<List<int>>(allianceslIstObj.AllianceList);
                var AllianceStatusObj= ConcurrentSingleton<NHManager>.Instance.Insert(alliancestatusObj);
                gangslist.Add(AllianceStatusObj.ID);

                RoleAlliance immortalsAlliance = new RoleAlliance() { RoleID= immortalsAllianceObj .RoleID,AllianceID= AllianceStatusObj.ID,AllianceJob= immortalsAllianceObj.AllianceJob,Reputation=0};
                Alliances alliances = new Alliances() {ID=1,AllianceList=Utility.Json.ToJson(gangslist) };
                ConcurrentSingleton<NHManager>.Instance.Update(alliances);
                ConcurrentSingleton<NHManager>.Instance.Update(immortalsAlliance);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(immortalsAlliance));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
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
