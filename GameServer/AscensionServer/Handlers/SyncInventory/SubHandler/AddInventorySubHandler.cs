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
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
            AscensionServer._Log.Info(">>>>>接收roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>接收背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
            bool existRing = ConcurrentSingleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);

            if (exist && existRing)
            {

                var ringServerArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Ring>(nHCriteriaRingID);

                if (InventoryObj.ID == ringServerArray.ID)
                {
                    //AscensionServer._Log.Info("ringarray" + ringServerArray.RingItems);
                    var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                    Dictionary<int, RingItemsDTO> posDict ;
                    foreach (var client_p in InventoryObj.RingItems)
                    {
                        var firstKey = 0;
                        posDict = new Dictionary<int, RingItemsDTO>();
                        firstKey = ServerDic.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2").Key;
                        if (!ServerDic.ContainsKey(client_p.Key))
                        {
                            
                            ServerDic.Add(client_p.Key, client_p.Value);
                            if (firstKey != 0)
                            {
                                //AscensionServer._Log.Info("<>" + firstKey);
                                var tempDict = ServerDic;
                                var indexOne = ServerDic.ToList().FindIndex(s => s.Key == client_p.Key);
                                var indexTwo = ServerDic.ToList().FindIndex(s => s.Key == firstKey);
                                //AscensionServer._Log.Info("<>" + indexOne);//-1
                                //AscensionServer._Log.Info("<>" + indexTwo);
                                for (int i = 0; i < ServerDic.Count; i++)
                                {
                                    int numb = i;
                                    if (indexOne == numb)
                                    {
                                        posDict.Add(firstKey, tempDict[firstKey]);
                                        continue;
                                    }
                                    if (indexTwo == numb)
                                    {
                                        posDict.Add(client_p.Key, tempDict[client_p.Key]);
                                        continue;
                                    }
                                    posDict.Add(ServerDic.ToList()[numb].Key, ServerDic.ToList()[numb].Value);
                                }
                            }
                        }
                        else
                        {
                            var severValue = ServerDic[client_p.Key];
                            if (client_p.Value.RingItemCount > 0)
                                severValue.RingItemCount += client_p.Value.RingItemCount;
                            if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                            if (severValue.RingItemTime != client_p.Value.RingItemTime)
                                severValue.RingItemTime = client_p.Value.RingItemTime;
                        }
                        ServerDic = posDict.Count == 0 ? ServerDic : posDict;
                        ConcurrentSingleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(ServerDic),  RingMagicDictServer = ringServerArray.RingMagicDictServer });
                    }
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}


