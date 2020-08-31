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
   public class GetAllianceMemberSubHandler : SyncAllianceMemberSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceMemberJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var allianceMemberObj = Utility.Json.ToObject<AllianceMemberDTO>(allianceMemberJson);
            NHCriteria nHCriteriallianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceMemberObj.AllianceID);
            var allianceMemberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceMember>(nHCriteriallianceMember);
            AscensionServer._Log.Info("发送的仙盟的所有成员" + allianceMemberTemp.Member);
            List<int> memberList = new List<int>();
            List<RoleAllianceDTO> allianceMembers = new List<RoleAllianceDTO>();
            List<NHCriteria > nHCriterias= new List<NHCriteria>();
            if (allianceMemberTemp != null)
            {
                if (!string.IsNullOrEmpty(allianceMemberTemp.Member))
                {
                    memberList = Utility.Json.ToObject<List<int>>(allianceMemberTemp.Member);
                    for (int i = 0; i < memberList.Count; i++)
                    {
                        NHCriteria nHCriteriMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", memberList[i]);
                        nHCriterias.Add(nHCriteriMember);
                        var MemberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriMember);
                        RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = MemberTemp.AllianceID, AllianceJob = MemberTemp.AllianceJob, JoinTime = MemberTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(MemberTemp.ApplyForAlliance), JoinOffline = MemberTemp.JoinOffline, Reputation = MemberTemp.Reputation, ReputationHistroy = MemberTemp.ReputationHistroy, ReputationMonth = MemberTemp.ReputationMonth, RoleID = MemberTemp.RoleID, RoleName = MemberTemp.RoleName };
                        allianceMembers.Add(roleAllianceDTO);
                    }

                    SetResponseData(() =>
                    {
                        AscensionServer._Log.Info("发送的仙盟的所有成员"+ Utility.Json.ToJson(allianceMembers));
                        SubDict.Add((byte)ParameterCode.AllianceMember, Utility.Json.ToJson(allianceMembers));
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
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriterias);
        }
    }
}