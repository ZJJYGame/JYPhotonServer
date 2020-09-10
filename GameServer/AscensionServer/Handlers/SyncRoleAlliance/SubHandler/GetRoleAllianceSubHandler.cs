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
    public class GetRoleAllianceSubHandler : SyncRoleAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var roleallianceObj = Utility.Json.ToObject<RoleAllianceDTO>
              (roleallianceJson);
            NHCriteria nHCriteriaroleAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceObj.RoleID);
            var roleallianceTemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriaroleAlliances);
            List<string> Alliancelist = new List<string>();



            if (roleallianceTemp!=null)
            {
                Utility.Debug.LogInfo("接受到的个人仙盟信息" + roleallianceJson);
                NHCriteria nHCriteriaAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", roleallianceTemp.AllianceID);
                var Role = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleallianceObj.RoleID);

                RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = roleallianceTemp.AllianceID, AllianceJob = roleallianceTemp.AllianceJob, JoinTime = roleallianceTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinOffline = roleallianceTemp.JoinOffline, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleID = roleallianceTemp.RoleID, RoleName = roleallianceTemp.RoleName,RoleSchool= roleallianceTemp.RoleSchool,RoleLevel= Role.RoleLevel };
                var allianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceStatus>(nHCriteriaAlliances).Result;
                Utility.Debug.LogInfo("2接受到的个人仙盟信息" + roleallianceJson);
                NHCriteria nHCriteriaAlliancesConstruction = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", roleallianceTemp.AllianceID);

                var allianceConstructionTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceConstruction>(nHCriteriaAlliancesConstruction);

                Alliancelist.Add(Utility.Json.ToJson(roleAllianceDTO));
                if (allianceConstructionTemp!=null)
                {
                    Alliancelist.Add(Utility.Json.ToJson(allianceConstructionTemp));
                }
                if (allianceTemp!=null)
                {
                    Alliancelist.Add(Utility.Json.ToJson(allianceTemp));
                }
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo("发送回去的的个人仙盟信息" + Utility.Json.ToJson(Alliancelist));
                    SubDict.Add((byte)ParameterCode.RoleAlliance, Utility.Json.ToJson(Alliancelist));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
                GameManager.ReferencePoolManager.Despawns(nHCriteriaroleAlliances, nHCriteriaAlliances, nHCriteriaAlliancesConstruction);
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
