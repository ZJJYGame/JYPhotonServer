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
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatus>
                (alliancestatusJson);
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var immortalsAllianceObj = Utility.Json.ToObject<RoleAllianceDTO> 
                (immortalsAllianceJson);

            AscensionServer._Log.Info("1获得的仙盟数据" + alliancestatusObj);
            NHCriteria nHCriteriaAllianceName = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);
            var alliance = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAllianceName);
            List<DataObject> Alliancelist = new List<DataObject>();
            if (alliance==null)
            {

                List<int> gangslist = new List<int>();
                NHCriteria nHCriteriaAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);
                var allianceslIstObj= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaAlliance);
                AscensionServer._Log.Info("2获得的仙盟数据" + alliancestatusObj.AllianceName);
                gangslist = Utility.Json.ToObject<List<int>>(allianceslIstObj.AllianceList);
                var AllianceStatusObj= ConcurrentSingleton<NHManager>.Instance.Insert(alliancestatusObj);
                gangslist.Add(AllianceStatusObj.ID);

                RoleAlliance immortalsAlliance = new RoleAlliance() { RoleID= immortalsAllianceObj .RoleID,AllianceID= AllianceStatusObj.ID,AllianceJob= immortalsAllianceObj.AllianceJob,Reputation=0};
                Alliances alliances = new Alliances() {ID=1,AllianceList=Utility.Json.ToJson(gangslist) };
                ConcurrentSingleton<NHManager>.Instance.Update(alliances);
                ConcurrentSingleton<NHManager>.Instance.Update(immortalsAlliance);
                Alliancelist.Add(immortalsAlliance);
                Alliancelist.Add(AllianceStatusObj);
                AscensionServer._Log.Info("发送的仙盟数据为" + Utility.Json.ToJson(Alliancelist));
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(Alliancelist));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
                ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAllianceName, nHCriteriaAlliance);
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
