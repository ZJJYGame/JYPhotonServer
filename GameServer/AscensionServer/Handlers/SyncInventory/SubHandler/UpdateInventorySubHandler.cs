using System;
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
    public class UpdateInventorySubHandler : SyncInventorySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
            AscensionServer._Log.Info(">>>>>更新roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>更新背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            Dictionary<int, RingItemsDTO> Dic;
            int serverInfoItemCount = 0;
            string severInfoItemTime = "0";
            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
            bool existRing = ConcurrentSingleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);

            if (exist)
            {
                if (existRing)
                {
                    var ringServerArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Ring>(nHCriteriaRingID);

                    if (InventoryObj.ID == ringServerArray.ID)
                    {
                        Dic = new Dictionary<int, RingItemsDTO>();
                        var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                        foreach (var server_p in ServerDic)
                        {
                            if (InventoryObj.RingItems.ContainsKey(server_p.Key))
                                continue;
                            Dic.Add(server_p.Key, server_p.Value);
                        }
                        foreach (var client_p in InventoryObj.RingItems)
                        {
                            if (ServerDic.ContainsKey(client_p.Key))
                            {
                                var severValue = ServerDic[client_p.Key];
                                if (severValue.RingItemCount > client_p.Value.RingItemCount)
                                {
                                    serverInfoItemCount = severValue.RingItemCount - client_p.Value.RingItemCount;
                                    client_p.Value.RingItemCount = serverInfoItemCount;
                                    client_p.Value.RingItemTime = severValue.RingItemTime;
                                    client_p.Value.RingItemAdorn = severValue.RingItemAdorn;
                                    AscensionServer._Log.Info("" + client_p.Value.RingItemTime);
                                    Dic.Add(client_p.Key, client_p.Value);
                                }
                                else
                                {
                                    Dic.Remove(client_p.Key);
                                }
                                ConcurrentSingleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(Dic) });
                            }
                            /*
                            else
                            {
                                Dic.Add(client_p.Key, client_p.Value);
                                Singleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(Dic) });
                            }*/
                        }
                        Owner.OpResponse.Parameters = Owner.ResponseData;
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    }
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}
