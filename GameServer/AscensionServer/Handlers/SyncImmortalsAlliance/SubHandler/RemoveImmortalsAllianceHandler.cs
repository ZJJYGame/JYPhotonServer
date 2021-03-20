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
    public class RemoveImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string allianceMemberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var allianceMemberObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceMemberJson);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            var roleselfObj = Utility.Json.ToObject<RoleDTO>(roleJson);

            var allianceMemberTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceMember>("AllianceID", allianceMemberObj.AllianceID);

            if (allianceMemberTemp != null)
            {
                Utility.Debug.LogError("解散仙盟" + allianceMemberTemp.AllianceID);
                if (!string.IsNullOrEmpty(allianceMemberTemp.Member))
                {
                    List<int> memberlist = new List<int>();
                    memberlist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.Member);
                    for (int i = 0; i < memberlist.Count; i++)
                    {
                        Utility.Debug.LogError("解散仙盟2" + allianceMemberTemp.AllianceID);
                        var roleObj = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", memberlist[i]);
                        roleObj.AllianceID = 0;
                        roleObj.AllianceJob = 4;
                        roleObj.Reputation = 0;
                        roleObj.ReputationHistroy = 0;
                        roleObj.ReputationMonth = 0;
                        roleObj.JoinTime = null;
                        NHibernateQuerier.Update(roleObj);
                    }
                }
                if (!string.IsNullOrEmpty(allianceMemberTemp.ApplyforMember))
                {
                    Utility.Debug.LogInfo("解散仙盟3" + allianceMemberTemp.AllianceID);
                    List<int> applylist = new List<int>();
                    applylist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);
                    for (int i = 0; i < applylist.Count; i++)
                    {
                        var roleObj = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", applylist[i]);
                        var applyList = Utility.Json.ToObject<List<int>>(roleObj.ApplyForAlliance);
                        applyList.Remove(allianceMemberObj.AllianceID);
                        roleObj.ApplyForAlliance = Utility.Json.ToJson(applyList);
                        NHibernateQuerier.Update(roleObj);
                    }
                }
                var alliances = AlliancelogicManager.Instance.GetNHCriteria<Alliances>("ID", 1);
                var alliancesList = Utility.Json.ToObject<List<int>>(alliances.AllianceList);
                alliancesList.Remove(allianceMemberObj.AllianceID);
                alliances.AllianceList = Utility.Json.ToJson(alliancesList);
                NHibernateQuerier.Update(alliances);
                Utility.Debug.LogInfo("解散仙盟4" + allianceMemberTemp.AllianceID);

                var allianceStatusObj = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceMemberObj.AllianceID);
                var allianceConstructionObj = AlliancelogicManager.Instance.GetNHCriteria<AllianceConstruction>("AllianceID", allianceMemberObj.AllianceID);
                NHibernateQuerier.Delete(allianceConstructionObj);
                NHibernateQuerier.Delete(allianceStatusObj);
                NHibernateQuerier.Delete(allianceMemberTemp);

                var roleallianceTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", roleselfObj.RoleID);
                RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID= roleallianceTemp.RoleID,RoleName= roleallianceTemp .RoleName,ApplyForAlliance=new List<int>()};
                SetResponseParamters(() =>
                   {
                       subResponseParameters.Add((byte)ParameterCode.Alliances, Utility.Json.ToJson(roleAllianceDTO));
                       operationResponse.ReturnCode = (short)ReturnCode.Success;
                   });
            }
            return operationResponse;
        }
    }
}


