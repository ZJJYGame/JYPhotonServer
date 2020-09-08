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


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceMemberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var allianceMemberObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceMemberJson);


            var allianceMemberTemp = AlliancelogicManager.Instance.GetNHCriteria<AllianceMember>("AllianceID", allianceMemberObj.AllianceID);

            if (allianceMemberTemp!=null)
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
                        roleObj.AllianceJob = 50;
                        ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);
                    }
                }
                if (!string.IsNullOrEmpty(allianceMemberTemp.ApplyforMember))
                {
                    Utility.Debug.LogError("解散仙盟3" + allianceMemberTemp.AllianceID);
                    List<int> applylist = new List<int>();
                    applylist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);
                    for (int i = 0; i < applylist.Count; i++)
                    {
                        var roleObj = AlliancelogicManager.Instance.GetNHCriteria<RoleAlliance>("RoleID", applylist[i]);
                        var applyList = Utility.Json.ToObject<List<int>>(roleObj.ApplyForAlliance);
                        applyList.Remove(allianceMemberObj.AllianceID);
                        roleObj.ApplyForAlliance = Utility.Json.ToJson(applyList);
                    await    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);

                    }
                }
                var alliances = AlliancelogicManager.Instance.GetNHCriteria<Alliances>("ID",1);
                var alliancesList = Utility.Json.ToObject<List<int>>(alliances.AllianceList);
                alliancesList.Remove(allianceMemberObj.AllianceID);
                alliances.AllianceList = Utility.Json.ToJson(alliancesList);
             await   ConcurrentSingleton<NHManager>.Instance.UpdateAsync(alliances);

                Utility.Debug.LogError("解散仙盟4" + allianceMemberTemp.AllianceID);

                var allianceStatusObj = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceMemberObj.AllianceID);

                var allianceConstructionObj = AlliancelogicManager.Instance.GetNHCriteria<AllianceConstruction>("AllianceID", allianceMemberObj.AllianceID);
              await  ConcurrentSingleton<NHManager>.Instance.DeleteAsync(allianceConstructionObj);
             await   ConcurrentSingleton<NHManager>.Instance.DeleteAsync(allianceStatusObj);
             await   ConcurrentSingleton<NHManager>.Instance.DeleteAsync(allianceMemberTemp);
                SetResponseData(() =>
                {
                    Utility.Debug.LogError("解散仙盟5");
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
