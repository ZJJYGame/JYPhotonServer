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

            NHCriteria nHCriteriaroleAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleAllianceObj.RoleID);
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
                    NHCriteria nHCriteriaroleAllianceMember = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceID", roleAllianceObj.ApplyForAlliance[i]);
                    var allianceMemberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceMember>(nHCriteriaroleAllianceMember);
                }


                 SetResponseData(() =>
                {
                    roleAllianceObj.ApplyForAlliance = applyList;
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(roleAllianceObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
