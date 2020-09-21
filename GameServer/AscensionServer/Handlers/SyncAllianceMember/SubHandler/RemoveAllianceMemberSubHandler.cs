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
    public class RemoveAllianceMemberSubHandler : SyncAllianceMemberSubHandler
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

            NHCriteria nHCriteriallianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceMemberObj.AllianceID);
            var allianceMemberTemp = NHibernateQuerier.CriteriaSelectAsync<AllianceMember>(nHCriteriallianceMember).Result;

            #region 待删
            List<NHCriteria> nHCriterias = new List<NHCriteria>();
            nHCriterias.Add(nHCriteriallianceMember);
            List<int> memberlist = new List<int>();
            if (allianceMemberTemp != null)
            {
                if (!string.IsNullOrEmpty(allianceMemberTemp.Member))
                {
                    for (int i = 0; i < allianceMemberObj.Member.Count; i++)
                    {
                        NHCriteria nHCriteriMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", allianceMemberObj.Member[i]);
                        nHCriterias.Add(nHCriteriMember);

                        var alliancestatus = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceMemberObj.AllianceID);
                        alliancestatus.AllianceNumberPeople -=1;
                       await NHibernateQuerier.UpdateAsync(alliancestatus);
                        var MemberTemp = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriMember).Result;
                        MemberTemp.AllianceID = 0;
                        MemberTemp.AllianceJob = 50;
                     await   NHibernateQuerier.UpdateAsync(MemberTemp);

                        memberlist = Utility.Json.ToObject<List<int>>(allianceMemberTemp.Member);
                        memberlist.Remove(allianceMemberObj.Member[i]);
                        allianceMemberTemp.Member = Utility.Json.ToJson(memberlist);
                     await   NHibernateQuerier.UpdateAsync(allianceMemberTemp);
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                }
                else
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
            }
            #endregion
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriterias);
        }
    }
}
