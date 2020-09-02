﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
using NHibernate.Criterion;

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
            Utility.Debug.LogInfo(">>>>>更新roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>更新背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.Json.ToObject<RoleRing>(InventoryRoleData);
            var InventoryObj = Utility.Json.ToObject<RingDTO>(InventoryData);

            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            NHCriteria nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", InventoryObj.ID);
            bool existRing = ConcurrentSingleton<NHManager>.Instance.Verify<Ring>(nHCriteriaRingID);
            Dictionary<int, RingItemsDTO> posDict;

            if (exist && existRing)
            {
                var ringServerArray = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Ring>(nHCriteriaRingID);

                if (InventoryObj.ID == ringServerArray.ID)
                {
                    var ServerDic = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                    var ServerMagicDic = Utility.Json.ToObject<Dictionary<int, int>>(ringServerArray.RingMagicDictServer);
                    var ServerDictAdorn = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingAdorn);

                    foreach (var client_p in InventoryObj.RingItems)
                    {
                        //if (!ServerDic.ContainsKey(client_p.Key))
                        //{
                        //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        //    continue;
                        //}
                        //else
                        //{
                        Utility.Debug.LogInfo("<>" + client_p.Key);
                        RingItemsDTO serverData = null;
                        if (ServerDic.TryGetValue(client_p.Key, out serverData) || ServerDictAdorn.TryGetValue(client_p.Key, out serverData))
                        {
                            if (serverData.RingItemCount >= client_p.Value.RingItemCount)
                            {
                                serverData.RingItemCount -= client_p.Value.RingItemCount;
                                serverData.RingItemTime = serverData.RingItemTime;
                                serverData.RingItemAdorn = client_p.Value.RingItemAdorn;
                                serverData.RingItemMax = serverData.RingItemMax;
                                if (client_p.Value.RingItemCount == serverData.RingItemMax)
                                    serverData.RingItemCount = 0;

                                if (client_p.Value.RingItemAdorn == "2" || client_p.Value.RingItemAdorn == "1"|| ServerDictAdorn.ContainsKey(client_p.Key))
                                {
                                    var firstAdorn = ServerDictAdorn.FirstOrDefault(q => Int32.Parse(q.Key.ToString().Substring(0, 5)) == Int32.Parse(client_p.Key.ToString().Substring(0, 5))).Key;
                                    Utility.Debug.LogInfo("<firstAdorn>" + firstAdorn);
                                    if (firstAdorn == 0 && !ServerDictAdorn.ContainsKey(client_p.Key))
                                        ServerDictAdorn.Add(client_p.Key, serverData);
                                    else
                                    {
                                        var firstKeyDefault = ServerDic.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;
                                        Utility.Debug.LogInfo("<firstKeyDefault>" + firstKeyDefault);
                                        ////DOTO
                                        if (firstKeyDefault != 0)
                                        {
                                            ServerDictAdorn[firstKeyDefault] = serverData;
                                            ServerDic = ServerDic.ToDictionary(k => k.Key == firstKeyDefault ? client_p.Key : k.Key, k => k.Value);
                                            ServerDictAdorn.Remove(firstAdorn);
                                        }
                                        else
                                        {
                                            Utility.Debug.LogInfo("<>" + firstAdorn);
                                            ServerDictAdorn[firstAdorn].RingItemAdorn = "0";
                                            if (!ServerDictAdorn.ContainsKey(firstAdorn))
                                                ServerDic.Add(firstAdorn, ServerDictAdorn[firstAdorn]);
                                            else if (!ServerDic.ContainsKey(firstAdorn))
                                                ServerDic.Add(firstAdorn, ServerDictAdorn[firstAdorn]);
                                            ServerDictAdorn.Remove(firstAdorn);
                                        }
                                    }
                                }

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
                        }
                    }
                    #region ob


                    /*
                    var firstKey = 0;
                    posDict = new Dictionary<int, RingItemsDTO>();
                    if (!ServerDic.ContainsKey(client_p.Key))
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        continue;
                    }
                    else
                    {
                        var serverData = ServerDic[client_p.Key];
                        firstKey = ServerDic.FirstOrDefault(q => q.Value.RingItemAdorn  == "1"|| q.Value.RingItemAdorn == "2").Key;
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
                            if (firstKey != 0)
                            {
                                Utility.Debug.LogInfo("<>" + firstKey);
                                //posDict.Clear();
                                var tempDict = ServerDic;
                                var indexOne = ServerDic.ToList().FindIndex(s => s.Key == client_p.Key);
                                //AscensionServer._Log.Info("<>" + indexOne);
                                var indexTwo = ServerDic.ToList().FindIndex(s => s.Key == firstKey);
                                //AscensionServer._Log.Info("<>" + indexTwo);
                                for (int i = 0; i < ServerDic.Count; i++)
                                {
                                    int numb = i;
                                    if (indexOne == numb)
                                    {
                                        posDict.Add(firstKey, tempDict[firstKey]);
                                        //AscensionServer._Log.Info("<>" + firstKey);
                                        continue;
                                    }
                                    if (indexTwo == numb)
                                    {
                                        posDict.Add(client_p.Key, tempDict[client_p.Key]);
                                        //AscensionServer._Log.Info("<>" + client_p.Key);
                                        continue;
                                    }
                                    posDict.Add(ServerDic.ToList()[numb].Key, ServerDic.ToList()[numb].Value);
                                }
                            }
                        }
                        else
                        {
                            ServerDic.Remove(client_p.Key);
                        }
                    }
                    Utility.Debug.LogInfo("<>" + posDict.Count);
                    Utility.Debug.LogInfo("<>" + ServerDic.Count);
                    ServerDic = posDict.Count == 0 ? ServerDic : posDict;
                    //var serverJsonDict = firstKey != 0 ? posDict : ServerDic;

                    */

                    #endregion
                    ConcurrentSingleton<NHManager>.Instance.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(ServerDic), RingMagicDictServer = Utility.Json.ToJson(ServerMagicDic), RingAdorn = Utility.Json.ToJson(ServerDictAdorn) });
                    // }
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }

            }
            else Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
        }
    }
}
