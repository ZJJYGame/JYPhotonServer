﻿using System;
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
    public  class UpdateApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);

            string allianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var allianceObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceJson);
            List<int> roleidList = new List<int>();
            roleidList = allianceObj.ApplyforMember;
            List<NHCriteria> NHCriterias = new List<NHCriteria>();
            Utility.Debug.LogInfo("进来的查找所有成员的数据 ");
            NHCriteria nHCriteriallianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceObj.AllianceID);
            NHCriterias.Add(nHCriteriallianceMember);
            NHCriteria nHCriterialliancer = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", allianceObj.AllianceID);
            NHCriterias.Add(nHCriterialliancer);
            var allianceMemberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceMember>(nHCriteriallianceMember).Result;
            var allianceStatusTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceStatus>(nHCriterialliancer).Result;
            List<int> applyList = new List<int>();
            List<int> memberList = new List<int>();
            List<RoleAllianceDTO> roleAllianceDTOs = new List<RoleAllianceDTO>();
            applyList = Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);
            memberList = Utility.Json.ToObject<List<int>>(allianceMemberTemp.Member);
            //判断帮派人数是否已满
            Utility.Debug.LogInfo("进来的查找所有成员的数据 " + roleidList[0]);
            if (allianceStatusTemp.AlliancePeopleMax> allianceStatusTemp.AllianceNumberPeople)
            {
                for (int i = 0; i < roleidList.Count; i++)
                {

                    NHCriteria nHCriteriRoleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleidList[i]);

                    NHCriterias.Add(nHCriteriRoleAlliance);
                    var roleAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriRoleAlliance).Result;

                    if (roleAllianceTemp.AllianceID == 0)
                    {
                        roleAllianceTemp.AllianceID = allianceMemberTemp.AllianceID;
                        //roleAllianceTemp.JoinOffline =
                        roleAllianceTemp.ApplyForAlliance = "[]";
                       await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAllianceTemp);

                        RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { RoleID = roleAllianceTemp.RoleID, ApplyForAlliance = new List<int>(), AllianceID = roleAllianceTemp.AllianceID, ReputationHistroy = roleAllianceTemp.ReputationHistroy, AllianceJob = roleAllianceTemp.AllianceJob, JoinOffline = roleAllianceTemp.JoinOffline, JoinTime = roleAllianceTemp.JoinTime, Reputation = roleAllianceTemp.Reputation, ReputationMonth = roleAllianceTemp.ReputationMonth, RoleName = roleAllianceTemp.RoleName,RoleSchool= roleAllianceTemp.RoleSchool };
                        roleAllianceDTOs.Add(roleAllianceDTO);

                        allianceStatusTemp.AllianceNumberPeople++;

                        applyList.Remove(roleidList[i]);

                        memberList.Add(roleidList[i]);
                        if (ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAllianceSkill>(nHCriteriRoleAlliance).Result==null)
                        {
                            RoleAllianceSkill roleAllianceSkill = new RoleAllianceSkill() { RoleID = roleAllianceTemp.RoleID };
                            await ConcurrentSingleton<NHManager>.Instance.InsertAsync(roleAllianceSkill);
                        }
                    }
                }
                allianceMemberTemp.ApplyforMember = Utility.Json.ToJson(applyList);
                allianceMemberTemp.Member = Utility.Json.ToJson(memberList);

                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceMemberTemp);
                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceStatusTemp);
            }
            if (roleAllianceDTOs.Count>0)
            {
                SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.ApplyForAlliance, Utility.Json.ToJson(roleAllianceDTOs));
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
            GameManager.ReferencePoolManager.Despawns(NHCriterias);
        }
    }
}
