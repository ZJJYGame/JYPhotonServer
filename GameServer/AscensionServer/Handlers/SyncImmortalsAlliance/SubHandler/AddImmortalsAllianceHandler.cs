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
    public class AddImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatus>
                (alliancestatusJson);
            string roleAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var roleAllianceObj = Utility.Json.ToObject<RoleAlliance> 
                (roleAllianceJson);

          
            NHCriteria nHCriteriaAllianceName = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);
            var alliance = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAllianceName);


            List<string> Alliancelist = new List<string>();
            NHCriteria nHCriteriaroleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleAllianceObj.RoleID);
            var roleAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriaroleAlliance);


            #region MySql数据模块
            if (alliance == null)
            {
                List<int> gangslist = new List<int>();
                NHCriteria nHCriteriaAllianceList = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", 1);
                var allianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaAllianceList);
                gangslist = Utility.Json.ToObject<List<int>>(allianceTemp.AllianceList);


                var allianceslIstObj =await ConcurrentSingleton<NHManager>.Instance.InsertAsync(alliancestatusObj);

                AllianceConstruction allianceConstruction = new AllianceConstruction() { AllianceID = allianceslIstObj.ID};
               await ConcurrentSingleton<NHManager>.Instance.InsertAsync(allianceConstruction);
                AllianceMember allianceMember = new AllianceMember() { AllianceID = allianceslIstObj.ID, ApplyforMember = Utility.Json.ToJson(new List<int>() { }) ,Member = Utility.Json.ToJson(new List<int>() { roleAllianceObj .RoleID}) };
              await  ConcurrentSingleton<NHManager>.Instance.InsertAsync(allianceMember);
                gangslist.Add(allianceslIstObj.ID);
                allianceTemp.AllianceList = Utility.Json.ToJson(gangslist);
                RoleAllianceSkill roleAllianceSkill = new RoleAllianceSkill() { RoleID= roleAllianceObj.RoleID };

                await ConcurrentSingleton<NHManager>.Instance.InsertAsync(roleAllianceSkill);
                await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceTemp);

                Alliancelist.Add(Utility.Json.ToJson(allianceslIstObj));
                if (roleAllianceTemp != null)
                {
                    roleAllianceTemp.RoleID = roleAllianceObj.RoleID;
                    roleAllianceTemp.AllianceID = allianceslIstObj.ID;
                    roleAllianceTemp.AllianceJob = roleAllianceObj.AllianceJob;
                    roleAllianceTemp.Reputation = roleAllianceObj.Reputation;
                  await  ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAllianceTemp);
                }
                else
                {
                    roleAllianceTemp = new RoleAlliance()
                    {
                        RoleID = roleAllianceObj.RoleID,
                        AllianceID = allianceslIstObj.ID,
                        AllianceJob = roleAllianceObj.AllianceJob,
                        Reputation = roleAllianceObj.Reputation,
                    };
                    ConcurrentSingleton<NHManager>.Instance.Insert(roleAllianceTemp);
                }
                RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO() { AllianceID = roleAllianceTemp.AllianceID, AllianceJob = roleAllianceTemp.AllianceJob, JoinTime = roleAllianceTemp.JoinTime, ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleAllianceTemp.ApplyForAlliance), JoinOffline = roleAllianceTemp.JoinOffline, Reputation = roleAllianceTemp.Reputation, ReputationHistroy = roleAllianceTemp.ReputationHistroy, ReputationMonth = roleAllianceTemp.ReputationMonth, RoleID = roleAllianceTemp.RoleID, RoleName = roleAllianceTemp.RoleName };
                Alliancelist.Add(Utility.Json.ToJson(roleAllianceDTO));
                Alliancelist.Add(Utility.Json.ToJson(allianceConstruction));
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(Alliancelist));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });

                #region  Redis模块
                await RedisHelper.String.StringSetAsync("Alliance", allianceTemp.AllianceList);
                await RedisHelper.Hash.HashSetAsync("AllianceConstruction", allianceConstruction.AllianceID.ToString(), allianceConstruction);
                await RedisHelper.Hash.HashSetAsync("AllianceMember", allianceMember.AllianceID.ToString(), allianceMember);
                await RedisHelper.Hash.HashSetAsync("RoleAlliance", roleAllianceDTO.RoleID.ToString(), roleAllianceDTO);
                await RedisHelper.Hash.HashSetAsync("AllianceStatus", allianceslIstObj.ID.ToString(), allianceslIstObj);
                #endregion
                GameManager.ReferencePoolManager.Despawns(nHCriteriaAllianceList, nHCriteriaAllianceName);
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            #endregion
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
