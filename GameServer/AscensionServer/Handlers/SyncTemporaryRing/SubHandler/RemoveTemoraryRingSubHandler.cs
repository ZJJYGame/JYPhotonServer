﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    public class RemoveTemoraryRingSubHandler: SyncTemporarySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var TemRingRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTemInventory) as string;
            AscensionServer._Log.Info(">>>>>Remove 临时背包" + TemRingRoleData + ">>>>>>>>>>>>>");
            var TemRingRoleObj = Utility.Json.ToObject<TemporaryRingDTO>(TemRingRoleData);
            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", TemRingRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                var ringServer = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<TemporaryRing>(nHCriteriaRoleID);
                var tempRing = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServer.RingItems);

                foreach (var temp in TemRingRoleObj.RingItems)
                {
                    if (!tempRing.ContainsKey(temp.Key))
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        continue;
                    }
                    else
                        tempRing.Remove(temp.Key);
                    ConcurrentSingleton<NHManager>.Instance.Update(new TemporaryRing() { RoleID = TemRingRoleObj.RoleID, RingItems = Utility.Json.ToJson(tempRing) });
                }
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}