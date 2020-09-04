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
    public class GetAllianceConstructionSubHandler : SyncAllianceConstructionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceConstructionJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceConstruction));

            var allianceConstructionObj = Utility.Json.ToObject<AllianceConstructionDTO>(allianceConstructionJson);
            NHCriteria nHCriteriallianceConstruction = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceConstructionObj.AllianceID);

            Utility.Debug.LogError("獲得的得到的"+ allianceConstructionJson);
            var allianceConstructionTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceConstruction>(nHCriteriallianceConstruction).Result;
            Utility.Debug.LogError("2獲得的得到的" + Utility.Json.ToJson(allianceConstructionTemp));
            if (allianceConstructionTemp != null)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
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
            GameManager.ReferencePoolManager.Despawns(nHCriteriallianceConstruction);
        }
    }
}
