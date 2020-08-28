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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var roleallianceObj = Utility.Json.ToObject<RoleAllianceDTO>
  (roleallianceJson);
            NHCriteria nHCriteriaroleAlliances = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleallianceObj.RoleID);
            var roleallianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriaroleAlliances).Result;
            List<int> memberlist = new List<int>();
            if (roleallianceTemp!=null)
            {
                NHCriteria nHCriteriaAlliances = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceID", roleallianceTemp.AllianceID);
                var allianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceMember>(nHCriteriaAlliances).Result;
                memberlist = Utility.Json.ToObject<List<int>>(allianceTemp.Member);
                memberlist.Remove(roleallianceObj.RoleID);
                allianceTemp.Member = Utility.Json.ToJson(memberlist);
                ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceTemp);

                roleallianceTemp.AllianceID = 0;
                roleallianceTemp.Reputation = 0;
                roleallianceTemp.ReputationHistroy = 0;
                roleallianceTemp.ReputationMonth = 0;
                ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleallianceTemp);
            }
        }
    }
}
