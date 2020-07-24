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
    public class UpdateHerbsFieldSubHandler : SyncHerbsFieldSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string herbsfieldJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobHerbsField));
            var hfObj = Utility.Json.ToObject<HerbsFieldDTO>(herbsfieldJson);
            NHCriteria nHCriteriahf = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", hfObj.RoleID);
            AscensionServer._Log.Info("接收到的霛田信息" + herbsfieldJson);
            var hfTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<HerbsField>(nHCriteriahf);
            List<HerbFieldStatus> hfList = new List<HerbFieldStatus>();
            if (hfTemp != null)
            {
                hfList = Utility.Json.ToObject<List<HerbFieldStatus>>(hfTemp.AllHerbs);
                if (hfList.Count<hfObj.AllHerbs[0].ArrayID)
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                        peer.SendOperationResponse(Owner.OpResponse, sendParameters);
                        ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriahf);
                        return;
                    });                  
                }else
                {
                    for (int i = 0; i < hfObj.AllHerbs.Count; i++)
                    {
                        hfList[hfObj.AllHerbs[i].ArrayID] = hfObj.AllHerbs[i];
                    }            
                }
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriahf);
        }
    }
}
