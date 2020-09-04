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

            if (allianceConstructionObj!=null)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionObj));
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
