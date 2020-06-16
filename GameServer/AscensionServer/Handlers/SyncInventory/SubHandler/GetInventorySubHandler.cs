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
using Newtonsoft.Json;

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
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Inventory) as string;
            AscensionServer._Log.Info(">>>>>接收roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>接收背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RingId", InventoryObj.RingId);
            bool existRing = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);

            if (exist)
            {
                var ringArray = Singleton<NHManager>.Instance.CriteriaGet<RoleRing>(nHCriteriaRoleID);
                if (existRing)
                {
                    var ringServerArray = Singleton<NHManager>.Instance.CriteriaGet<Ring>(nHCriteriaRingID);
                    Owner.ResponseData.Add((byte)ObjectParameterCode.Inventory, ringServerArray.RingItems);
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}
