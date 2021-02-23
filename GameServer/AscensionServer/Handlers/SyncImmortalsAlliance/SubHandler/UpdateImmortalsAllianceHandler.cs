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
    public class UpdateImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string immortalsAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var immortalsAllianceObj = Utility.Json.ToObject<AllianceStatusDTO>
                (immortalsAllianceJson);
            var allianceMemberTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", immortalsAllianceObj.ID);
          
            if (allianceMemberTemp!=null)
            {
                if (allianceMemberTemp.AllianceName == immortalsAllianceObj.AllianceName)
                {
                    allianceMemberTemp.Manifesto = immortalsAllianceObj.Manifesto;
                    NHibernateQuerier.Update(allianceMemberTemp);
                    SetResponseParamters(() =>
                    {
                        subResponseParameters.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceMemberTemp));
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }else
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
            }
            else
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            return operationResponse;
        }
    }
}


