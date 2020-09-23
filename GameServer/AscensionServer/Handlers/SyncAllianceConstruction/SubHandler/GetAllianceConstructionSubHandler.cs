﻿using System;
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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            {
                var dict = ParseSubParameters(operationRequest);
                string allianceConstructionJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceConstruction));

                var allianceConstructionObj = Utility.Json.ToObject<AllianceConstructionDTO>(allianceConstructionJson);
                NHCriteria nHCriteriallianceConstruction = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceConstructionObj.AllianceID);


                Utility.Debug.LogError("獲得的得到的仙盟建設" + allianceConstructionJson);
                var allianceConstructionTemp = NHibernateQuerier.CriteriaSelectAsync<AllianceConstruction>(nHCriteriallianceConstruction).Result;
                Utility.Debug.LogError("2獲得的得到的仙盟建設" + Utility.Json.ToJson(allianceConstructionTemp));
                if (allianceConstructionTemp != null)
                {
                    SetResponseParamters(() =>
                    {
                        subResponseParameters.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
                        operationResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseParamters(() =>
                    {
                        operationResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                GameManager.ReferencePoolManager.Despawns(nHCriteriallianceConstruction);
                return operationResponse;
            }
        }
    }
}
