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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string allianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var allianceObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceJson);
            List<int> roleidList = new List<int>();
            roleidList = allianceObj.ApplyforMember;
            NHCriteria nHCriteriallianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceObj.AllianceID);
            var alliancememberTemp =NHibernateQuerier.CriteriaSelectAsync<AllianceMember>(nHCriteriallianceMember).Result;

            List<NHCriteria> NHCriterias = new List<NHCriteria>();
            NHCriterias.Add(nHCriteriallianceMember);


            List<int> applyList = new List<int>();
            List<int> memberList = new List<int>();
            applyList = Utility.Json.ToObject<List<int>>(alliancememberTemp.ApplyforMember);
            Utility.Debug.LogError("数据库储存的成员的数据 " + Utility.Json.ToJson(applyList));
            for (int i = 0; i < roleidList.Count; i++)
            {

                NHCriteria nHCriteriRoleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleidList[i]);

                NHCriterias.Add(nHCriteriRoleAlliance);
                var roleAllianceTemp = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriRoleAlliance).Result;
                if (roleAllianceTemp.AllianceID == 0)
                {

                    var roleApplyList = Utility.Json.ToObject<List<int>>(roleAllianceTemp.ApplyForAlliance);
                    roleApplyList.Remove(allianceObj.AllianceID);
                    roleAllianceTemp.ApplyForAlliance = Utility.Json.ToJson(roleApplyList);
                  await   NHibernateQuerier.UpdateAsync(roleAllianceTemp);


                    Utility.Debug.LogError("进来的查找所有成员的数据 " + roleidList[i]);
                    applyList.Remove(roleidList[i]);
                }
            }
            alliancememberTemp.ApplyforMember=Utility.Json.ToJson(applyList);
          await  NHibernateQuerier.UpdateAsync(alliancememberTemp);
            SetResponseData(() =>
            {
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
                GameManager.ReferencePoolManager.Despawns(NHCriterias);
            }
        }
    }
