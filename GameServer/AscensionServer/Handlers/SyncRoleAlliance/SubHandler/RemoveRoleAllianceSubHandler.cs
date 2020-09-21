﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;


namespace AscensionServer
{
    public class RemoveRoleAllianceSubHandler : SyncRoleAllianceSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var roleallianceObj = Utility.Json.ToObject<RoleAllianceDTO>(roleallianceJson);
            Utility.Debug.LogError("储存的成员" + roleallianceJson);
            NHCriteria nHCriteriaroleAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceObj.RoleID);
            var roleallianceTemp = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaroleAlliances).Result;
            List<int> memberlist = new List<int>();
            if (roleallianceTemp!=null)
            {
                if (roleallianceTemp.AllianceJob!=1)
                {
                    var alliancestatus = AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", roleallianceTemp.AllianceID);
                    alliancestatus.AllianceNumberPeople -= 1;
                  await  NHibernateQuerier.UpdateAsync(alliancestatus);
                    NHCriteria nHCriteriaAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", roleallianceTemp.AllianceID);
                    var allianceTemp = NHibernateQuerier.CriteriaSelectAsync<AllianceMember>(nHCriteriaAlliances).Result;
                    memberlist = Utility.Json.ToObject<List<int>>(allianceTemp.Member);
                    memberlist.Remove(roleallianceObj.RoleID);
                    allianceTemp.Member = Utility.Json.ToJson(memberlist);
                  await  NHibernateQuerier.UpdateAsync(allianceTemp);
                    roleallianceTemp.AllianceJob = 50;
                    roleallianceTemp.AllianceID = 0;
                    roleallianceTemp.Reputation = 0;
                    roleallianceTemp.ReputationHistroy = 0;
                    roleallianceTemp.ReputationMonth = 0;
                  await   NHibernateQuerier.UpdateAsync(roleallianceTemp);

                    RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = roleallianceTemp.AllianceID, AllianceJob = roleallianceTemp.AllianceJob, JoinTime = roleallianceTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleallianceTemp.ApplyForAlliance), JoinOffline = roleallianceTemp.JoinOffline, Reputation = roleallianceTemp.Reputation, ReputationHistroy = roleallianceTemp.ReputationHistroy, ReputationMonth = roleallianceTemp.ReputationMonth, RoleID = roleallianceTemp.RoleID, RoleName = roleallianceTemp.RoleName };
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RoleAlliance, Utility.Json.ToJson(roleAllianceDTO));
                        Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                    });
                    GameManager.ReferencePoolManager.Despawns(nHCriteriaAlliances, nHCriteriaroleAlliances);
                }
                else
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
                    });
                }

            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
        }
    }
}
