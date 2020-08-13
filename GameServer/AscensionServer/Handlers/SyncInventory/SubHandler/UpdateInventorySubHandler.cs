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

            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
            bool existRing = ConcurrentSingleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);


            if (exist && existRing)
            {
                var ringServerArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Ring>(nHCriteriaRingID);

                if (InventoryObj.ID == ringServerArray.ID)
                {
                    var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                    var ServerMagicDic = Utility.Json.ToObject<Dictionary<int, int>>(ringServerArray.RingMagicDictServer);
                    foreach (var client_p in InventoryObj.RingItems)
                    {
                        if (!ServerDic.ContainsKey(client_p.Key))
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                            continue;
                        }
                        else
                        {
                            var serverData = ServerDic[client_p.Key];
                            if (serverData.RingItemCount > client_p.Value.RingItemCount)
                            {
                                serverData.RingItemCount -= client_p.Value.RingItemCount;
                                serverData.RingItemTime = serverData.RingItemTime;
                                serverData.RingItemAdorn = client_p.Value.RingItemAdorn;
                                if (ServerMagicDic.Count != 0)
                                {

                                    for (int i = 0; i < ServerMagicDic.Count; i++)
                                    {
                                        if (ServerMagicDic.Values.ToList()[i] == -1 && client_p.Value.RingItemAdorn == "2")
                                        {
                                            ServerMagicDic[i] = client_p.Key;
                                            break;
                                        }
                                        else if (ServerMagicDic.Values.ToList()[i] == client_p.Key && client_p.Value.RingItemAdorn == "0")
                                        {
                                            ServerMagicDic[i] = -1;
                                            break;
                                        }
                                    }
                                }
                            }
                            else ServerDic.Remove(client_p.Key);
                        }
                        ConcurrentSingleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(ServerDic),RingMagicDictServer = Utility.Json.ToJson(ServerMagicDic) });
                    }
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
           
            }
            else Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}
