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
    public class RemoveApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string allianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var allianceObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceJson);
            List<int> roleidList = new List<int>();
            roleidList = allianceObj.ApplyforMember;
            Utility.Debug.LogError("进来的查找所有成员的数据 "+Utility.Json.ToJson(roleidList));

            NHCriteria nHCriteriallianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceObj.AllianceID);


            var alliancememberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceMember>(nHCriteriallianceMember).Result;

            List<NHCriteria> NHCriterias = new List<NHCriteria>();
            NHCriterias.Add(nHCriteriallianceMember);

            List<int> applyList = new List<int>();
            List<int> memberList = new List<int>();
            for (int i = 0; i < roleidList.Count; i++)
            {

                NHCriteria nHCriteriRoleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleidList[i]);

                NHCriterias.Add(nHCriteriRoleAlliance);
                var roleAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriRoleAlliance).Result;
                Utility.Debug.LogError("进来的查找所有成员的数据 " + roleAllianceTemp.RoleID);
                if (roleAllianceTemp.AllianceID == 0)
                {

                    var roleApplyList = Utility.Json.ToObject<List<int>>(roleAllianceTemp.ApplyForAlliance);
                    roleApplyList.Remove(allianceObj.AllianceID);
                    roleAllianceTemp.ApplyForAlliance = Utility.Json.ToJson(roleApplyList);
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAllianceTemp);

                    applyList = Utility.Json.ToObject<List<int>>(alliancememberTemp.ApplyforMember);

                    applyList.Remove(roleidList[i]);
                }
            }
            alliancememberTemp.ApplyforMember=Utility.Json.ToJson(applyList);
            ConcurrentSingleton<NHManager>.Instance.UpdateAsync(alliancememberTemp);
            SetResponseData(() =>
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
                GameManager.ReferencePoolManager.Despawns(NHCriterias);
            }


        }
    }
