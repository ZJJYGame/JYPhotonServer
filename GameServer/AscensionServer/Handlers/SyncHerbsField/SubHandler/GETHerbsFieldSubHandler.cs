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
    public class GetHerbsFieldSubHandler : SyncHerbsFieldSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string herbsfieldJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobHerbsField));
            var hfObj = Utility.Json.ToObject<HerbsField>(herbsfieldJson);

            NHCriteria nHCriteriahf = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", hfObj.RoleID);
            Utility.Debug.LogInfo("接收到的霛田信息"+ herbsfieldJson);
            var hfTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<HerbsField>(nHCriteriahf);
            if (hfTemp!=null)
            {
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo("發送的霛田信息" + herbsfieldJson);
                    HerbsFieldDTO herbsFieldDTO = new HerbsFieldDTO() { RoleID= hfTemp .RoleID,jobLevel= hfTemp .jobLevel};
                    herbsFieldDTO.AllHerbs = Utility.Json.ToObject<List<HerbFieldStatus>>(hfTemp.AllHerbs);
                    SubDict.Add((byte)ParameterCode.JobHerbsField, Utility.Json.ToJson(herbsFieldDTO));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriahf);
        }
    }
}
