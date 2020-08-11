﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
   public  class AddForgeSubHandler:SyncForgeSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string forgeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobForge));
            var forgeObj = Utility.Json.ToObject<ForgeDTO>(forgeJson);
            NHCriteria nHCriteriaforge = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", forgeObj.RoleID);
            var forgeTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Forge>(nHCriteriaforge);
            HashSet<int> forgeHash = new HashSet<int>();
            if (forgeTemp!=null)
            {
                if (string.IsNullOrEmpty(forgeTemp.Recipe_Array))
                {
                    forgeTemp.Recipe_Array = Utility.Json.ToJson(forgeObj.Recipe_Array);
                   
                    ConcurrentSingleton<NHManager>.Instance.Update(forgeTemp);
                }
                else
                {
                   forgeHash = Utility.Json.ToObject<HashSet<int>>(forgeTemp.Recipe_Array);
                    forgeHash.Add(forgeObj.Recipe_Array.First());
                    forgeTemp.Recipe_Array = Utility.Json.ToJson(forgeHash);
                    ConcurrentSingleton<NHManager>.Instance.Update(forgeTemp);
                }
                SetResponseData(() =>
                {
                    forgeObj = new ForgeDTO() { RoleID = forgeTemp.RoleID, JobLevel = forgeTemp.JobLevel, JobLevelExp = forgeTemp.JobLevelExp, Recipe_Array = forgeHash };
                    SubDict.Add((byte)ParameterCode.JobForge, Utility.Json.ToJson(forgeObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaforge);
        }

    }
}