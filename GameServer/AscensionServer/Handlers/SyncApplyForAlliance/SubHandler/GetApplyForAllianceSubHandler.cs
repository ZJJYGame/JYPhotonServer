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
    public class GetApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancememberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var alliancememberObj = Utility.Json.ToObject<AllianceMemberDTO>(alliancememberJson);

            List<ApplyForAllianceDTO> applyForAllianceList = new List<ApplyForAllianceDTO>();
            List<int> applyForList = new List<int>();
            NHCriteria nHCriteriaalliancemember = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceID", alliancememberObj.AllianceID);
            var alliancememberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceMember>(nHCriteriaalliancemember);
            applyForList = Utility.Json.ToObject<List<int>>(alliancememberTemp.ApplyforMember);
            for (int i = 0; i < applyForList.Count; i++)
            {
             var roleObj= AlliancelogicManager.Instance.GetNHCriteria<Role>("RoleID", applyForList[i]);
                var schoolObj = AlliancelogicManager.Instance.GetNHCriteria<RoleSchool>("RoleID", applyForList[i]);
                var gongfaObj = AlliancelogicManager.Instance.GetNHCriteria<RoleGongFa>("RoleID", applyForList[i]);
                applyForAllianceList.Add(AlliancelogicManager.Instance.JointDate(roleObj, schoolObj));
            }


            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(applyForAllianceList));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaalliancemember);
        }
    }
}
