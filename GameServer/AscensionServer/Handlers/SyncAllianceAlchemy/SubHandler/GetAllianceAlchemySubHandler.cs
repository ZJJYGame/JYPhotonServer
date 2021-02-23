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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string allianceAlchemyNumJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAllianceAlchemy));
            var allianceAlchemyNumObj = Utility.Json.ToObject<AllianceAlchemyNumDTO>(allianceAlchemyNumJson);

            var redisKey = RedisData.ReidsDataProcessing.InsertName("AllianceAlchemyNum", allianceAlchemyNumObj.RoleID);

            var content= RedisData.ReidsDataProcessing.GetData(redisKey);

            if (String.IsNullOrEmpty(content))
            {
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.RoleAllianceAlchemy, Utility.Json.ToJson(allianceAlchemyNumObj));
                    Utility.Debug.LogError("1获得的兑换的丹药"+ Utility.Json.ToJson(allianceAlchemyNumObj));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseParamters(() =>
                {
                    Utility.Debug.LogError("2获得的兑换的丹药" + content);
                    subResponseParameters.Add((byte)ParameterCode.RoleAllianceAlchemy, content);
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            return operationResponse;
        }
    }
}


