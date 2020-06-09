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
    public class AddInventorySubHandler : SyncInventorySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
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
            if (exist)
            {
                var ringArray = Singleton<NHManager>.Instance.CriteriaGet<RoleRing>(nHCriteriaRoleID);
                NHCriteria nHCriteriaRingID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RingId", InventoryObj.RingId);
                bool existRing = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);
                if (existRing)
                {
                    //string serverInfoItem = "";
                    int serverInfoItemCount = 0;
                    string severInfoItemAdorn = "";
                    var ringServerArray = Singleton<NHManager>.Instance.CriteriaGet<Ring>(nHCriteriaRingID);
                    foreach (var n in Utility.ToObject<List<int>>(ringArray.RingIdArray))
                    {
                        if (n == ringServerArray.ID)
                        {
                            AscensionServer._Log.Info("ringarray" + ringServerArray.RingItems);
                            var ServerDic = Utility.ToObject<HashSet<RingItemsDTO>>(ringServerArray.RingItems);
                            foreach (var p in ServerDic)
                            {
                                foreach (var client_p in InventoryObj.RingItems)
                                {
                                    if (p.RingItemId == client_p.RingItemId)
                                    {
                                        serverInfoItemCount = p.RingItemCount;
                                        if (p.RingItemCount != client_p.RingItemCount)
                                        {
                                            serverInfoItemCount = p.RingItemCount + client_p.RingItemCount;
                                            client_p.RingItemCount = serverInfoItemCount;
                                        }
                                        severInfoItemAdorn = p.RingItemAdorn;
                                        if (p.RingItemAdorn != client_p.RingItemAdorn)
                                        {
                                            severInfoItemAdorn = client_p.RingItemAdorn;
                                            client_p.RingItemAdorn = severInfoItemAdorn;
                                        }
                                        //AscensionServer._Log.Info("  ><<<<<<<<<<<<<" + Utility.ToJson(InventoryObj.RingItems));
                                        Singleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.ToJson(InventoryObj.RingItems) });
                                    }
                                    else
                                    {
                                        ServerDic.Add(client_p);
                                        Singleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.ToJson(ServerDic) });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
