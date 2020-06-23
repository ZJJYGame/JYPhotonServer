using System;
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
    public class GetInventorySubHandler : SyncInventorySubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
            AscensionServer._Log.Info(">>>>>接收roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>接收背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            Dictionary<int,int> idRing = new Dictionary<int,int>();
            Ring ring = null ;
            if (exist)
            {
                var ringArray = Singleton<NHManager>.Instance.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
                /*
                if (string.IsNullOrEmpty(ringArray.RingIdArray))
                {
                    ring = Singleton<NHManager>.Instance.Insert<Ring>(new Ring() { RingId = InventoryObj.RingId, RingItems = Utility.Json.ToJson(new Dictionary<int, RingItemsDTO>()) });
                    idRing.Add(ring.ID, ring.RingAdorn);
                    Singleton<NHManager>.Instance.Update<RoleRing>(new RoleRing() { RoleID = InventoryRoleObj.RoleID, RingIdArray = Utility.Json.ToJson(idRing) });
                }*/
                NHCriteria nHCriteriaRingID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
                bool existRing = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);
                if (existRing)
                {
                    var ringServerArray = Singleton<NHManager>.Instance.CriteriaSelect<Ring>(nHCriteriaRingID);
                    Owner.ResponseData.Add((byte)ParameterCode.Inventory, ringServerArray.RingItems);
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
/*
               if (!string.IsNullOrEmpty(ringArray.RingIdArray))
               {
                   idRing = Utility.Json.ToObject<List<int>>(ringArray.RingIdArray);
                   foreach (var item in idRing)
                   {
                       if (item != InventoryObj.ID)
                       {
                           idRing.Add(ring.ID);
                       }
                   }
               }
               else*/

