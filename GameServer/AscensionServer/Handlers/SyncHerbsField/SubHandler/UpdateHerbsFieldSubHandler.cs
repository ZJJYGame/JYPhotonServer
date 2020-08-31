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
            NHCriteria nHCriteriahf = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", hfObj.RoleID);
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
                        GameManager.ReferencePoolManager.Despawns(nHCriteriahf);
                        return;
                    });                  
                }else
                {
                    for (int i = 0; i < hfObj.AllHerbs.Count; i++)
                    {
                        hfList[hfObj.AllHerbs[i].ArrayID].FieldLevel+= hfObj.AllHerbs[i].FieldLevel;
                        hfList[hfObj.AllHerbs[i].ArrayID].HerbsID= hfObj.AllHerbs[i].HerbsID;
                        hfList[hfObj.AllHerbs[i].ArrayID].HerbsGrowthValue = hfObj.AllHerbs[i].HerbsGrowthValue;
                        if (hfObj.AllHerbs[i].IsStratPlant)
                        {
                            hfList[hfObj.AllHerbs[i].ArrayID].plantingTime = hfObj.AllHerbs[i].plantingTime;
                        }
                        ConcurrentSingleton<NHManager>.Instance.Update<HerbsField>(new HerbsField() {RoleID= hfTemp.RoleID,jobLevel= hfTemp.jobLevel,AllHerbs= Utility.Json.ToJson(hfList) });
                        for (int j = 0; j < hfObj.AllHerbs.Count; j++)
                        {
                            hfObj.AllHerbs[j].IsStratPlant = false;
                        }
                        SetResponseData(() =>
                        {                     
                            SubDict.Add((byte)ParameterCode.RoleSchool, Utility.Json.ToJson(hfObj));
                            AscensionServer._Log.Info("的霛田信息" + Utility.Json.ToJson(hfObj));
                            Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                        });
                    }            
                }
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriahf);
        }
    }
}
