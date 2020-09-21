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
    public class RemoveInventorySubHandler : SyncInventorySubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            ResetResponseData(operationRequest);
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
            Utility.Debug.LogInfo(">>>>>背包移除 roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>背包移除的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
            bool existRing = NHibernateQuerier.Verify<Ring>(nHCriteriaRingID);
            if (exist && existRing)
            {
                var ringServerArray = NHibernateQuerier.CriteriaSelect<Ring>(nHCriteriaRingID);
                if (InventoryObj.ID == ringServerArray.ID)
                {
                    var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                    var ServerMagicDic = Utility.Json.ToObject<Dictionary<int, int>>(ringServerArray.RingMagicDictServer);
                    var ServerDictAdorn = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingAdorn);
                    foreach (var client_p in InventoryObj.RingItems)
                    {
                        if (!ServerDic.ContainsKey(client_p.Key))
                        {
                            Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
                            break;
                        }
                        else
                        {
                            int NowID = 0;
                            while (true)
                            {
                                var randomNumer = new Random().Next(3001, 4000);
                                Utility.Debug.LogInfo("<randomNumer>" + randomNumer);
                                if (client_p.Key.ToString().Length == 6 || client_p.Key.ToString().Length == 7 || client_p.Key.ToString().Length == 8)
                                    NowID = Int32.Parse(client_p.Key.ToString() + randomNumer.ToString().Substring(0, 3));
                                else if (client_p.Key.ToString().Length > 8)
                                    NowID = client_p.Key + randomNumer;
                                else
                                    NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                if (!ServerDic.ContainsKey(NowID) && !ServerDictAdorn.ContainsKey(NowID))
                                {
                                    ServerDic = ServerDic.ToDictionary(k => k.Key == client_p.Key ? NowID : k.Key, k => k.Value);
                                    ServerDic[NowID].RingItemCount = 0;
                                    ServerDic[NowID].RingItemAdorn = "0";
                                    break;
                                }
                            }
                            //ServerDic.Remove(client_p.Key);

                        }
                    }
                    NHibernateQuerier.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(ServerDic), RingMagicDictServer = Utility.Json.ToJson(ServerMagicDic), RingAdorn = Utility.Json.ToJson(ServerDictAdorn) });
                    Owner.OpResponseData.Parameters = Owner.ResponseData;
                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                }

            }
            else Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}
