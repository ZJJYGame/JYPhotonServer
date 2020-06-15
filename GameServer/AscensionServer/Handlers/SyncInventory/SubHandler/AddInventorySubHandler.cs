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
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RingId", InventoryObj.RingId);
            bool existRing = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);
            Dictionary<int, RingItemsDTO> Dic;
            int serverInfoItemCount = 0;
            string severInfoItemAdorn = "";
            if (exist)
            {
                var ringArray = Singleton<NHManager>.Instance.CriteriaGet<RoleRing>(nHCriteriaRoleID);

                if (existRing)
                {

                    var ringServerArray = Singleton<NHManager>.Instance.CriteriaGet<Ring>(nHCriteriaRingID);
                    foreach (var n in Utility.Json.ToObject<List<int>>(ringArray.RingIdArray))
                    {
                        if (n == ringServerArray.ID)
                        {
                            Dic = new Dictionary<int, RingItemsDTO>();
                            AscensionServer._Log.Info("ringarray" + ringServerArray.RingItems);
                            var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                            AscensionServer._Log.Info(ServerDic.Count);
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
                                    serverInfoItemCount = severValue.RingItemCount;
                                    if (severValue.RingItemCount != client_p.Value.RingItemCount)
                                    {
                                        serverInfoItemCount = severValue.RingItemCount + client_p.Value.RingItemCount;
                                        client_p.Value.RingItemCount = serverInfoItemCount;
                                    }
                                    severInfoItemAdorn = severValue.RingItemAdorn;
                                    if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                    {
                                        severInfoItemAdorn = client_p.Value.RingItemAdorn;
                                        client_p.Value.RingItemAdorn = severInfoItemAdorn;
                                    }

                                    Dic.Add(client_p.Key, client_p.Value);
                                    Singleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(Dic) });
                                }
                                else
                                {
                                    Dic.Add(client_p.Key, client_p.Value);
                                    Singleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(Dic) });
                                }
                            }
                        }
                    }
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}