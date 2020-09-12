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
    public class UpdateAllianceSigninSubHandler : SyncAllianceSigninSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceSigninJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceSignin));
            var allianceSigninObj = Utility.Json.ToObject<AllianceSigninDTO>(allianceSigninJson);

            var roleallianceTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", allianceSigninObj.RoleID);
            var allianceTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceSigninObj.AllianceID);

            var allianceConstructionTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceConstruction>("AllianceID", allianceSigninObj.AllianceID);
            List<string> signinList = new List<string>();
            if (roleallianceTemp!=null && allianceTemp != null&& allianceConstructionTemp!=null)
            {
                roleallianceTemp.Reputation += allianceSigninObj.RoleContribution;
                roleallianceTemp.ReputationHistroy += allianceSigninObj.RoleContribution;
                roleallianceTemp.ReputationMonth += allianceSigninObj.RoleContribution;

                allianceTemp.Popularity += allianceSigninObj.Popularity;
                allianceConstructionTemp.AllianceAssets += allianceSigninObj.AllianceSpiritStone;
                var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceTemp.RoleID);
                RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = roleallianceTemp.AllianceID, AllianceJob = roleallianceTemp.AllianceJob, JoinTime = roleallianceTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinOffline = roleallianceTemp.JoinOffline, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleID = roleallianceTemp.RoleID, RoleName = roleallianceTemp.RoleName, RoleSchool = roleallianceTemp.RoleSchool, RoleLevel = Role.RoleLevel };

                signinList.Add(Utility.Json.ToJson(roleAllianceDTO));
                signinList.Add(Utility.Json.ToJson(allianceTemp));
                signinList.Add(Utility.Json.ToJson(allianceConstructionTemp));
                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleallianceTemp);
                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceTemp);
                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceConstructionTemp);
                SetResponseData(() =>
                {
                    Utility.Debug.LogError("发送回去的兑换弹药的请求数据" + Utility.Json.ToJson(signinList));
                    SubDict.Add((byte)ParameterCode.AllianceSignin, Utility.Json.ToJson(signinList));
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
        }
    }
}
