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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
            Utility.Debug.LogInfo(">>>>>接收roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>接收背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleDTO>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);
            NHCriteria nHCriteriaRoleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            bool existRing = false;
            NHCriteria nHCriteriaRingID = null;
            if (exist)
            {
                var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriaRoleID);
                nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
                existRing = NHibernateQuerier.Verify<Ring>(nHCriteriaRingID);
            }
            switch (InventoryRoleObj.InventoryInstructions)
            {
                case InventoryInstructions.GetData:
                    InventoryManager.GetDataCmd(InventoryRoleObj.RoleID, InventoryObj, nHCriteriaRingID);
                    break;
                case InventoryInstructions.AddData:
                    InventoryManager.AddDataCmd(InventoryRoleObj.RoleID, InventoryObj, nHCriteriaRingID);
                    break;
                case InventoryInstructions.UpdateData:
                    InventoryManager.UdpateCmd(InventoryRoleObj.RoleID, InventoryObj, nHCriteriaRingID);
                    break;
                case InventoryInstructions.RemoveData:
                    InventoryManager.RemoveCmd(InventoryRoleObj.RoleID, InventoryObj, nHCriteriaRingID);
                    break;
                case InventoryInstructions.SortingData:
                    InventoryManager.SortingCmd(InventoryRoleObj.RoleID, InventoryObj, nHCriteriaRingID);
                    break;
            }
            #region ob
            /*
        if (exist && existRing)
        {
            SetResponseParamters(() =>
            {
                var ringServerArray = NHibernateQuerier.CriteriaSelect<Ring>(nHCriteriaRingID);
                subResponseParameters.Add((byte)ParameterCode.Inventory, ringServerArray.RingItems);
                subResponseParameters.Add((byte)ParameterCode.RoleRingMagic, ringServerArray.RingMagicDictServer);
                subResponseParameters.Add((byte)ParameterCode.RoleTemInventory, ringServerArray.RingAdorn);
                operationResponse.Parameters = subResponseParameters;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
            });
        }
        else
        {
            SetResponseParamters(() =>
            {
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            });
        }*/
            #endregion
            CosmosEntry.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}



