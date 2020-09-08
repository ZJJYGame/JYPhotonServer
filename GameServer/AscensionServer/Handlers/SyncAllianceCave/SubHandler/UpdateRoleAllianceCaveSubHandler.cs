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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
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

                    await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleallianceTemp);
                    await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAssetsTemp);

                    RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID = roleallianceTemp.RoleID, AllianceID = roleallianceTemp.AllianceID, JoinOffline = roleallianceTemp.JoinOffline, AllianceJob = roleallianceTemp.AllianceJob, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinTime = roleallianceTemp.JoinTime, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleName = roleallianceTemp.RoleName, RoleSchool = roleallianceTemp.RoleSchool };

                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RoleAllianceCave, Utility.Json.ToJson(roleAllianceDTO));
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
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
