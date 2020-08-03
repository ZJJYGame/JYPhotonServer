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

namespace AscensionServer.Handlers.SyncPuppet.SubHandler
{
    public class GetPuppetSubHandler : SyncPuppetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string puppetJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobPuppet));
            var puppetObj = Utility.Json.ToObject<PuppetDTO>(puppetJson);
            NHCriteria nHCriteriapuppetObj = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", puppetObj.RoleID);
            //AscensionServer._Log.Info("得到的锻造配方" );
            var puppettemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Puppet>(nHCriteriapuppetObj);
            if (puppettemp != null)
            {
                if (!string.IsNullOrEmpty(puppettemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.JobPuppet, Utility.Json.ToJson(puppettemp));

                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    //AscensionServer._Log.Info("得到的锻造配方"+ Utility.Json.ToJson(Frogetemp));
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobPuppet, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriapuppetObj);
        }
    }
}
