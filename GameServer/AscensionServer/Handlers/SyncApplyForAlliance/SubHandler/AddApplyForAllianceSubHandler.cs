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
    public class AddApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancememberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var alliancememberObj = Utility.Json.ToObject<AllianceMemberDTO>(alliancememberJson);

            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(roleJson);

            NHCriteria nHCriteriaallianceApplyFor = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceID", alliancememberObj.AllianceID);
            var allianceApplyForTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceMember>(nHCriteriaallianceApplyFor);
            List<int> applyList = new List<int>();
            if (!string.IsNullOrEmpty(allianceApplyForTemp.ApplyforMember))
            {
                applyList = Utility.Json.ToObject<List<int>>(allianceApplyForTemp.ApplyforMember);
                applyList.Add(roleObj.RoleID);
                var roleTemp = AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", roleObj.RoleID);
                var schoolTempj = AlliancelogicManager.Instance.GetNHCriteria<RoleSchool>("RoleID", roleObj.RoleID);
                var gongfaTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleGongFa>("RoleID", roleObj.RoleID);
                ApplyForAllianceDTO applyForAllianceDTO = AlliancelogicManager.Instance.JointDate(roleTemp, schoolTempj);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(applyForAllianceDTO));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
