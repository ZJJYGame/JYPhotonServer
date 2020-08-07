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
    public class AddPuppetSubHandler : SyncPuppetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string puppetJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobPuppet));
            var puppetObj = Utility.Json.ToObject<PuppetDTO>(puppetJson);
            NHCriteria nHCriteriaPuppet = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", puppetObj.RoleID);
            var puppetTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alchemy>(nHCriteriaPuppet);
            HashSet<int> puppetHash = new HashSet<int>();
            if (puppetTemp!=null)
            {
                if (string.IsNullOrEmpty(puppetTemp.Recipe_Array))
                {
                    puppetTemp.Recipe_Array = Utility.Json.ToJson(puppetObj.Recipe_Array);
                    ConcurrentSingleton<NHManager>.Instance.Update(puppetTemp);
                }
                else
                {
                    puppetHash = Utility.Json.ToObject<HashSet<int>>(puppetTemp.Recipe_Array);
                    puppetHash.Add(puppetObj.Recipe_Array.First());
                    puppetTemp.Recipe_Array = Utility.Json.ToJson(puppetHash);
                    ConcurrentSingleton<NHManager>.Instance.Update(puppetTemp);
                }
                SetResponseData(() =>
                {
                    puppetObj = new PuppetDTO() { RoleID = puppetTemp.RoleID, JobLevel = puppetTemp.JobLevel, JobLevelExp = puppetTemp.JobLevelExp, Recipe_Array = puppetHash };

                    SubDict.Add((byte)ParameterCode.JobPuppet, puppetObj);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;

                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaPuppet);
        }
    }
}
