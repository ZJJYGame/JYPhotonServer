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
    public class UpdateRoleAllianceCaveSubHandler : SyncRoleAllianceCaveSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string allianceCaveJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var allianceCaveObj = Utility.Json.ToObject<RoleAllianceDTO>(allianceCaveJson);

            string roleAssetsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAssets));
            var roleAssetsObj = Utility.Json.ToObject<RoleAssetsDTO>(roleAssetsJson);

            var roleallianceTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", allianceCaveObj.RoleID);
            var roleAssetsTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAssets>("RoleID", roleAssetsObj.RoleID);

            if (roleallianceTemp != null&& roleAssetsTemp!=null)
            {
                if (roleallianceTemp.Reputation >= allianceCaveObj.Reputation && roleAssetsTemp.SpiritStonesLow >= roleAssetsObj.SpiritStonesLow)
                {
                    roleallianceTemp.Reputation -= allianceCaveObj.Reputation;
                    roleAssetsTemp.SpiritStonesLow -= roleAssetsObj.SpiritStonesLow;

                    NHibernateQuerier.Update(roleallianceTemp);
                    NHibernateQuerier.Update(roleAssetsTemp);
                    RedisHelper.Hash.HashSet<RoleAssets>("RoleAssets", roleAssetsObj.RoleID.ToString(), roleAssetsTemp);
                    var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceTemp.RoleID);
                    RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID = roleallianceTemp.RoleID, AllianceID = roleallianceTemp.AllianceID, JoinOffline = roleallianceTemp.JoinOffline, AllianceJob = roleallianceTemp.AllianceJob, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinTime = roleallianceTemp.JoinTime, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleName = roleallianceTemp.RoleName,  RoleLevel = Role.RoleLevel };
                    SetResponseParamters(() =>
                    {
                        subResponseParameters.Add((byte)ParameterCode.RoleAllianceCave, Utility.Json.ToJson(roleAllianceDTO));
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
            return operationResponse;
        }
    }
}


