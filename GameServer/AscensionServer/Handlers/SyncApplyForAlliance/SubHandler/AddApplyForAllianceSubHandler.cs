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
            string roleAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(roleAllianceJson);
            AscensionServer._Log.Info("收到的加入仙盟的请求"+ roleAllianceJson);
            NHCriteria nHCriteriaroleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleAllianceObj.RoleID);
            var roleAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriaroleAlliance);
            List<int> applyList = new List<int>();
            if (!string.IsNullOrEmpty(roleAllianceTemp.ApplyForAlliance))
            {
                applyList = Utility.Json.ToObject<List<int>>(roleAllianceTemp.ApplyForAlliance);
                for (int i = 0; i < roleAllianceObj.ApplyForAlliance.Count; i++)
                {
                    applyList.Add(roleAllianceObj.ApplyForAlliance[i]);
                    roleAllianceTemp.ApplyForAlliance = Utility.Json.ToJson(applyList);
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAllianceTemp);
                    NHCriteria nHCriteriaroleAllianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", roleAllianceObj.ApplyForAlliance[i]);
                    var allianceMemberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceMember>(nHCriteriaroleAllianceMember);
                var applyer=    Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);
                    applyer.Add(roleAllianceObj.RoleID);
                    allianceMemberTemp.ApplyforMember = Utility.Json.ToJson(applyer);
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceMemberTemp);
                }


                 SetResponseData(() =>
                {
                    roleAllianceObj.ApplyForAlliance = applyList;
                    SubDict.Add((byte)ParameterCode.ApplyForAlliance, Utility.Json.ToJson(roleAllianceObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
