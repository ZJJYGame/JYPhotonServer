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

namespace AscensionServer
{
    public class GetAllianceAlchemySubHandler : SyncAllianceAlchemySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceAlchemyNumJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAllianceAlchemy));
            var allianceAlchemyNumObj = Utility.Json.ToObject<AllianceAlchemyNumDTO>(allianceAlchemyNumJson);

            var redisKey = RedisData.Initialize.InsertName("AllianceAlchemyNum", allianceAlchemyNumObj.RoleID);

            var content= RedisData.Initialize.GetData(redisKey);

            if (String.IsNullOrEmpty(content))
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RoleAllianceAlchemy, Utility.Json.ToJson(allianceAlchemyNumObj));
                    Utility.Debug.LogError("1获得的兑换的丹药"+ Utility.Json.ToJson(allianceAlchemyNumObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Utility.Debug.LogError("2获得的兑换的丹药" + content);
                    SubDict.Add((byte)ParameterCode.RoleAllianceAlchemy, content);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
