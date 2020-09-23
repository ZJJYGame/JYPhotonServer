using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateRoleAllianceSubHandler : SyncRoleAllianceSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = ParseSubParameters(operationRequest);
            string roleallianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var roleallianceObj = Utility.Json.ToObject<RoleAllianceDTO>(roleallianceJson);
            Utility.Debug.LogError("储存的成员" + roleallianceJson);

            NHCriteria nHCriteriaroleAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceObj.RoleID);
            var roleallianceTemp = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaroleAlliances).Result;
            if (roleallianceTemp!=null)
            {
                roleallianceTemp.Reputation += roleallianceObj.Reputation;
                roleallianceTemp.ReputationHistroy += roleallianceObj.ReputationHistroy;
                roleallianceTemp.ReputationMonth += roleallianceObj.ReputationMonth;
                roleallianceTemp.AllianceJob = roleallianceObj.AllianceJob;
                if (roleallianceTemp.Reputation >= 0 && roleallianceTemp.ReputationHistroy >= 0 && roleallianceTemp.ReputationMonth >= 0)
                {
                   NHibernateQuerier.Update(roleallianceTemp);
                    var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceObj.RoleID);
                    RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() {RoleID= roleallianceTemp.RoleID,AllianceID= roleallianceTemp.AllianceID,JoinOffline= roleallianceTemp.JoinOffline,AllianceJob= roleallianceTemp.AllianceJob,ApplyForAlliance= Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance),JoinTime= roleallianceTemp.JoinTime,Reputation= roleallianceTemp.Reputation,ReputationHistroy= roleallianceTemp.ReputationHistroy,ReputationMonth= roleallianceTemp.ReputationMonth,RoleName= roleallianceTemp.RoleName,RoleSchool= roleallianceTemp.RoleSchool,RoleLevel= Role.RoleLevel };

                    SetResponseParamters(() =>
                    {
                        subResponseParameters.Add((byte)ParameterCode.RoleAlliance, Utility.Json.ToJson(roleAllianceDTO));
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
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaroleAlliances);
            return operationResponse;
        }
    }
}
