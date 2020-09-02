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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceMemberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var allianceMemberObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceMemberJson);

            var allianceMemberTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceMember>("ID", allianceMemberObj.AllianceID);


            if (allianceMemberTemp!=null)
            {
                List<int> memberlist = new List<int>();
                memberlist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.Member);
                List<int> applylist = new List<int>();
                applylist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);

                for (int i = 0; i < memberlist.Count; i++)
                {
                    var roleObj = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", memberlist[i]);
                    roleObj.AllianceID = 0;
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);
                }
                for (int i = 0; i < applylist.Count; i++)
                {
                    var roleObj = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", applylist[i]);
                    var applyList = Utility.Json.ToObject<List<int>>(roleObj.ApplyForAlliance);
                    applyList.Remove(allianceMemberObj.AllianceID);
                    roleObj.ApplyForAlliance = Utility.Json.ToJson(applyList);
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);

                }
                var alliances = AlliancelogicManager.Instance.GetNHCriteria<Alliances>("ID", allianceMemberObj.AllianceID);
                var alliancesList = Utility.Json.ToObject<List<int>>(alliances.AllianceList);
                alliancesList.Remove(allianceMemberObj.AllianceID);
                alliances.AllianceList = Utility.Json.ToJson(alliancesList);
                ConcurrentSingleton<NHManager>.Instance.DeleteAsync(alliances);

                var allianceStatusObj = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceMemberObj.AllianceID);
                ConcurrentSingleton<NHManager>.Instance.DeleteAsync(allianceStatusObj);
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }


            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
