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
    public class AddHerbsFieldSubHandler : SyncHerbsFieldSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string herbsfieldJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobHerbsField));
            var hfObj = Utility.Json.ToObject<HerbsFieldDTO>(herbsfieldJson);


            NHCriteria nHCriteriahf = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", hfObj.RoleID);
            AscensionServer._Log.Info("接收到的霛田信息" + herbsfieldJson);
            var hfTemp = Singleton<NHManager>.Instance.CriteriaSelect<HerbsField>(nHCriteriahf);
            List<HerbFieldStatus> hfList = new List<HerbFieldStatus>();
            if (hfTemp!=null)
            {
                if (!string.IsNullOrEmpty(hfTemp.AllHerbs))
                {
                    hfList = Utility.Json.ToObject<List<HerbFieldStatus>>(hfTemp.AllHerbs);
                    if (!string.IsNullOrEmpty(Utility.Json.ToJson(hfObj.AllHerbs)))
                    {
                        for (int i = 0; i < hfObj.AllHerbs.Count; i++)
                        {
                            hfList.Add(hfObj.AllHerbs[i]);
                        }
                        hfTemp.AllHerbs = Utility.Json.ToJson(hfList);
                    }
                    else
                    {
                        hfList = hfObj.AllHerbs;
                    }
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriahf);
        }
    }
}
