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
            var immortalsAllianceObj = Utility.Json.ToObject<AlliancesDTO>
                (immortalsAllianceJson);
            var name = RedisData.Initialize.InsertName("(ALLIANCE_LIST", immortalsAllianceObj.ID);
            var content = RedisData.Initialize.GetData(name);

            List<int> alliances = new List<int>();
            List<NHCriteria> nhcriteriaList = new List<NHCriteria>();
            List<ImmortalsAlliance> ImmortalsAllianceList = new List<ImmortalsAlliance>();
            if (string.IsNullOrEmpty(content))
            {

                NHCriteria nHCriteriaimmortalsAllianceslist= ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);

                var immortalsAllianceslistTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaimmortalsAllianceslist);

                alliances = Utility.Json.ToObject<List<int>>(immortalsAllianceslistTemp.AllianceList);
                if (immortalsAllianceObj.AllIndex< alliances.Count)
                {
                    for (int i = immortalsAllianceObj.Index; i < immortalsAllianceObj.AllIndex; i++)
                    {
                        NHCriteria nHCriteriaimmortalsAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", i);
                        var immortalsAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ImmortalsAlliance>(nHCriteriaimmortalsAlliance);
                        ImmortalsAllianceList.Add(immortalsAllianceTemp);
                        nhcriteriaList.Add(nHCriteriaimmortalsAlliance);
                    }
                }
                else
                {
                    for (int i = immortalsAllianceObj.Index; i < alliances.Count; i++)
                    {
                        NHCriteria nHCriteriaimmortalsAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", i);
                        var immortalsAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ImmortalsAlliance>(nHCriteriaimmortalsAlliance);
                        ImmortalsAllianceList.Add(immortalsAllianceTemp);
                        nhcriteriaList.Add(nHCriteriaimmortalsAlliance);
                    }
                }

                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(ImmortalsAllianceList));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
        }
    }
}
