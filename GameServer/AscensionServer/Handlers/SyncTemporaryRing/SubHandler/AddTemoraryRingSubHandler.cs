﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;

namespace AscensionServer
{
    public class AddTemoraryRingSubHandler: SyncTemporarySubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var TemRingRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleTemInventory) as string;
            Utility.Debug.LogInfo(">>>>>Add 临时背包" + TemRingRoleData + ">>>>>>>>>>>>>");
            var TemRingRoleObj = Utility.Json.ToObject<TemporaryRingDTO>(TemRingRoleData);
            NHCriteria nHCriteriaRoleID =GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", TemRingRoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                var ringServer = NHibernateQuerier.CriteriaSelect<TemporaryRing>(nHCriteriaRoleID);
                var tempRing = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServer.RingItems);

                foreach (var temp in TemRingRoleObj.RingItems)
                {
                    if (!tempRing.ContainsKey(temp.Key))
                        tempRing.Add(temp.Key, temp.Value);
                    else
                    {
                        var tempValue =  tempRing[temp.Key];
                        if (temp.Value.RingItemCount > 0)
                            tempValue.RingItemCount += temp.Value.RingItemCount;
                        if (tempValue.RingItemTime != temp.Value.RingItemTime)
                            tempValue.RingItemTime = temp.Value.RingItemTime;
                        if (tempValue.RingItemAdorn != temp.Value.RingItemAdorn)
                            tempValue.RingItemAdorn = temp.Value.RingItemAdorn;
                    }
                    NHibernateQuerier.Update(new TemporaryRing() { RoleID = TemRingRoleObj.RoleID, RingItems = Utility.Json.ToJson(tempRing) });

                }
                Owner.OpResponseData.Parameters = Owner.ResponseData;
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}
