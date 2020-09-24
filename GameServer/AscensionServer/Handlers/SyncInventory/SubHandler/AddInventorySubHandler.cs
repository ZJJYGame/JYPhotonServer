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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        /// <summary>
        /// 需要count
        /// </summary>
        /// <param name="_ItemHeld"></param>
        /// <param name="_ItemMax"></param>
        /// <returns></returns>
        int NumberCountMethod(int _ItemHeld, int _ItemMax)
        {
            return _ItemHeld / _ItemMax;
        }
        /// <summary>
        /// 剩余的个数
        /// </summary>
        /// <param name="_ItemHeld"></param>
        /// <param name="_ItemMax"></param>
        /// <returns></returns>
        int HeldMethod(int _ItemHeld, int _ItemMax)
        {
            return _ItemHeld % _ItemMax;
        }
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var InventoryRoleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Inventory) as string;
			
            Utility.Debug.LogInfo(">>>>>背包添加roleId" + InventoryRoleData + ">>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>背包添加的数据" + InventoryData + ">>>>>>>>>>>>>");

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
                    var ServerDict = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingItems);
                    var ServerDictAdorn = Utility.Json.ToObject<Dictionary<int, RingItemsDTO>>(ringServerArray.RingAdorn);
                    //Dictionary<int, RingItemsDTO> posDict ;
                    foreach (var client_p in InventoryObj.RingItems)
                    {
                        //if (firstKeyDefault != 0)
                        //{
                        //    ///替换键值对
                        //    //ServerDict = ServerDict.ToDictionary(k => k.Key == client_p.Key ? 1 : k.Key, k => k.Value);
                        //    //dict = dict.ToDictionary(k => k.Key == "abc" ? "abce" : k.Key, k => k.Value);
                        //}
                        if (!ServerDict.ContainsKey(client_p.Key) && !ServerDictAdorn.ContainsKey(client_p.Key))
                        {
                            if (client_p.Value.RingItemCount > client_p.Value.RingItemMax)
                            {
                                int held = client_p.Value.RingItemCount;
                                var remainder = HeldMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax);
                                int numberCount = remainder == 0 || client_p.Value.RingItemMax == 1 ? NumberCountMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax) : NumberCountMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax) + 1;
                                for (int o = 0; o < numberCount; o++)
                                {
                                    int NowID = client_p.Key;
                                    while (true)
                                    {
                                        var randomNumer = new Random().Next(1000, 3000);
                                        
                                        if (!ServerDict.ContainsKey(NowID)&&!ServerDictAdorn.ContainsKey(NowID))
                                            break;
                                        NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                    }
                                    int numbCount = held > client_p.Value.RingItemMax ? client_p.Value.RingItemMax : remainder;
                                    RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = numbCount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn };
                                    var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                    if (firstKeyDefault != 0)
                                    {
                                        ServerDict[firstKeyDefault] = Items;
                                        ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                    }
                                    else
                                        ServerDict.Add(NowID, Items);
                                    if (client_p.Value.RingItemMax == 1 || remainder == 0)
                                        continue;
                                    held -= client_p.Value.RingItemMax;
                                }
                            }
                            else
                            {
                                #region 重复

                                ///TODO
                                int serverItemKey = ServerDict.Keys.ToList().Find(z => Int32.Parse(z.ToString().Substring(0, 5)) == client_p.Key && ServerDict[z].RingItemCount < ServerDict[z].RingItemMax);
                                if (serverItemKey != 0)
                                {
                                    if(ServerDict[serverItemKey].RingItemCount + client_p.Value.RingItemCount <= ServerDict[serverItemKey].RingItemMax)
                                    {
                                        var severValue = ServerDict[serverItemKey];
                                        if (client_p.Value.RingItemCount > 0)
                                            severValue.RingItemCount += client_p.Value.RingItemCount;
                                        if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                            severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                                        if (severValue.RingItemTime != client_p.Value.RingItemTime)
                                            severValue.RingItemTime = client_p.Value.RingItemTime;
                                        if (severValue.RingItemMax != client_p.Value.RingItemMax)
                                            severValue.RingItemMax = client_p.Value.RingItemMax;
                                        if (severValue.RingItemType != client_p.Value.RingItemType)
                                            severValue.RingItemType = client_p.Value.RingItemType;
                                    }
                                    else
                                    {

                                        var severValue = ServerDict[serverItemKey];
                                        int Amount = (severValue.RingItemCount + client_p.Value.RingItemCount) - client_p.Value.RingItemMax;
                                        severValue.RingItemCount = client_p.Value.RingItemMax;
                                        if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                            severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                                        if (severValue.RingItemTime != client_p.Value.RingItemTime)
                                            severValue.RingItemTime = client_p.Value.RingItemTime;
                                        if (severValue.RingItemMax != client_p.Value.RingItemMax)
                                            severValue.RingItemMax = client_p.Value.RingItemMax;
                                        if (severValue.RingItemType != client_p.Value.RingItemType)
                                            severValue.RingItemType = client_p.Value.RingItemType;
                                        Utility.Debug.LogInfo("<Amount>" + Amount);

                                        if (Amount > client_p.Value.RingItemMax)
                                        {
                                            int held = Amount;
                                            var remainder = HeldMethod(Amount, client_p.Value.RingItemMax);
                                            int numberCount = remainder == 0 || client_p.Value.RingItemMax == 1 ? NumberCountMethod(Amount, client_p.Value.RingItemMax) : NumberCountMethod(Amount, client_p.Value.RingItemMax) + 1;
                                            for (int o = 0; o < numberCount; o++)
                                            {
                                                int NowID = client_p.Key;
                                                while (true)
                                                {
                                                    var randomNumer = new Random().Next(1000, 3000);
                                                    if (!ServerDict.ContainsKey(NowID) && !ServerDictAdorn.ContainsKey(NowID))
                                                        break;
                                                    NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                                }
                                                int numbCount = held > client_p.Value.RingItemMax ? client_p.Value.RingItemMax : remainder;
                                                RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = numbCount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn, RingItemType = client_p.Value.RingItemType };
                                                var firstKeyTrueDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                                if (firstKeyTrueDefault != 0)
                                                {
                                                    ServerDict[firstKeyTrueDefault] = Items;
                                                    ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyTrueDefault ? NowID : k.Key, k => k.Value);
                                                }
                                                else
                                                    ServerDict.Add(NowID, Items);
                                                if (client_p.Value.RingItemMax == 1 || remainder == 0)
                                                    continue;
                                                held -= client_p.Value.RingItemMax;
                                            }
                                        }
                                        else
                                        {
                                            int NowID = client_p.Key;
                                            while (true)
                                            {
                                                var randomNumer = new Random().Next(1000, 3000);
                                                if (!ServerDict.ContainsKey(NowID) && !ServerDictAdorn.ContainsKey(NowID))
                                                    break;
                                                NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                            }
                                            Utility.Debug.LogInfo($"<NowID>{NowID}");

                                            RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = Amount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn, RingItemType = client_p.Value.RingItemType };
                                            var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                            if (firstKeyDefault != 0)
                                            {
                                                ServerDict[firstKeyDefault] = Items;
                                                ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                            }
                                            else
                                                ServerDict.Add(NowID, Items);
                                        }

                                    }

                                }
                                else
                                {
                                    var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;
                                    if (firstKeyDefault != 0)
                                    {
                                        ServerDict[firstKeyDefault] = client_p.Value;
                                        ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? client_p.Key : k.Key, k => k.Value);
                                    }
                                    else
                                        ServerDict.Add(client_p.Key, client_p.Value);
                                }
                            }
                            #endregion

                        }
                        else
                        {
                            int serverItemKey = ServerDict.Keys.ToList().Find(z => Int32.Parse(z.ToString().Substring(0, 5)) == client_p.Key && ServerDict[z].RingItemCount < ServerDict[z].RingItemMax);//(q => Int32.Parse(q.Key.ToString().Substring(0, 5)) == client_p.Key).Key;

                           Utility.Debug.LogInfo("<keylen>" + serverItemKey);

                            if (serverItemKey == 0)
                            {
                                if (client_p.Value.RingItemCount > client_p.Value.RingItemMax)
                                {
                                    int held = client_p.Value.RingItemCount;
                                    var remainder = HeldMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax);
                                    int numberCount = remainder == 0 || client_p.Value.RingItemMax == 1 ? NumberCountMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax) : NumberCountMethod(client_p.Value.RingItemCount, client_p.Value.RingItemMax) + 1;
                                    for (int o = 0; o < numberCount; o++)
                                    {
                                        int NowID = 0;
                                        while (true)
                                        {
                                            var randomNumer = new Random().Next(1000, 3000);
                                            NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                            if (!ServerDict.ContainsKey(NowID)&&!ServerDictAdorn.ContainsKey(NowID))
                                                break;
                                        }
                                        int numbCount = held > client_p.Value.RingItemMax ? client_p.Value.RingItemMax : remainder;
                                        RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = numbCount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn, RingItemType = client_p.Value.RingItemType };
                                        var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                        if (firstKeyDefault != 0)
                                        {
                                            ServerDict[firstKeyDefault] = Items;
                                            ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                        }
                                        else
                                            ServerDict.Add(NowID, Items);
                                        if (client_p.Value.RingItemMax == 1 || remainder == 0)
                                            continue;
                                        held -= client_p.Value.RingItemMax;
                                    }
                                }
                                else
                                {
                                    int NowID = 0;
                                    while (true)
                                    {
                                        var randomNumer = new Random().Next(1000, 3000);
                                        NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                        if (!ServerDict.ContainsKey(NowID)&& !ServerDictAdorn.ContainsKey(NowID))
                                            break;
                                    }
                                    Utility.Debug.LogInfo($"<NowID>{NowID}");

                                    RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = client_p.Value.RingItemCount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn , RingItemType = client_p.Value.RingItemType };
                                    var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                    if (firstKeyDefault != 0)
                                    {
                                        ServerDict[firstKeyDefault] = Items;
                                        ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                    }
                                    else
                                        ServerDict.Add(NowID, Items);
                                }
                            }
                            else
                            {
                                if (ServerDict[serverItemKey].RingItemCount + client_p.Value.RingItemCount <= ServerDict[serverItemKey].RingItemMax)
                                {
                                    var severValue = ServerDict[serverItemKey];
                                    if (client_p.Value.RingItemCount > 0)
                                        severValue.RingItemCount += client_p.Value.RingItemCount;
                                    if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                        severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                                    if (severValue.RingItemTime != client_p.Value.RingItemTime)
                                        severValue.RingItemTime = client_p.Value.RingItemTime;
                                    if (severValue.RingItemMax != client_p.Value.RingItemMax)
                                        severValue.RingItemMax = client_p.Value.RingItemMax;
                                    if (severValue.RingItemType != client_p.Value.RingItemType)
                                        severValue.RingItemType = client_p.Value.RingItemType;
                                }
                                else if (ServerDict[serverItemKey].RingItemCount + client_p.Value.RingItemCount > ServerDict[serverItemKey].RingItemMax)
                                {
                                    var severValue = ServerDict[serverItemKey];
                                    int Amount = (severValue.RingItemCount + client_p.Value.RingItemCount) - client_p.Value.RingItemMax;
                                    severValue.RingItemCount = client_p.Value.RingItemMax;
                                    if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                                        severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                                    if (severValue.RingItemTime != client_p.Value.RingItemTime)
                                        severValue.RingItemTime = client_p.Value.RingItemTime;
                                    if (severValue.RingItemMax != client_p.Value.RingItemMax)
                                        severValue.RingItemMax = client_p.Value.RingItemMax;
                                    if (severValue.RingItemType != client_p.Value.RingItemType)
                                        severValue.RingItemType = client_p.Value.RingItemType;
                                    Utility.Debug.LogInfo("<Amount>" + Amount);

                                    if (Amount > client_p.Value.RingItemMax)
                                    {
                                        int held = Amount;
                                        var remainder = HeldMethod(Amount, client_p.Value.RingItemMax);
                                        int numberCount = remainder == 0 || client_p.Value.RingItemMax == 1 ? NumberCountMethod(Amount, client_p.Value.RingItemMax) : NumberCountMethod(Amount, client_p.Value.RingItemMax) + 1;
                                        for (int o = 0; o < numberCount; o++)
                                        {
                                            int NowID = 0;
                                            while (true)
                                            {
                                                var randomNumer = new Random().Next(1000, 3000);
                                                NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                                if (!ServerDict.ContainsKey(NowID)&& !ServerDictAdorn.ContainsKey(NowID))
                                                    break;
                                            }
                                            int numbCount = held > client_p.Value.RingItemMax ? client_p.Value.RingItemMax : remainder;
                                            RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = numbCount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn, RingItemType = client_p.Value.RingItemType };
                                            var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                            if (firstKeyDefault != 0)
                                            {
                                                ServerDict[firstKeyDefault] = Items;
                                                ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                            }
                                            else
                                                ServerDict.Add(NowID, Items);
                                            if (client_p.Value.RingItemMax == 1 || remainder == 0)
                                                continue;
                                            held -= client_p.Value.RingItemMax;
                                        }
                                    }
                                    else
                                    {
                                        int NowID = 0;
                                        while (true)
                                        {
                                            var randomNumer = new Random().Next(1000, 3000);
                                            NowID = Int32.Parse(client_p.Key.ToString() + randomNumer);
                                            if (!ServerDict.ContainsKey(NowID)&& !ServerDictAdorn.ContainsKey(NowID))
                                                break;
                                        }
                                        Utility.Debug.LogInfo($"<NowID>{NowID}");

                                        RingItemsDTO Items = new RingItemsDTO() { RingItemTime = client_p.Value.RingItemTime, RingItemCount = Amount, RingItemMax = client_p.Value.RingItemMax, RingItemAdorn = client_p.Value.RingItemAdorn, RingItemType = client_p.Value.RingItemType };
                                        var firstKeyDefault = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2" || q.Value.RingItemCount == 0).Key;

                                        if (firstKeyDefault != 0)
                                        {
                                            ServerDict[firstKeyDefault] = Items;
                                            ServerDict = ServerDict.ToDictionary(k => k.Key == firstKeyDefault ? NowID : k.Key, k => k.Value);
                                        }
                                        else
                                            ServerDict.Add(NowID, Items);
                                    }
                                }
                            }
                        }
                    }
                    #region 
                    /*

                 foreach (var client_p in InventoryObj.RingItems)
                 {
                     var firstKey = 0;
                     posDict = new Dictionary<int, RingItemsDTO>();
                     firstKey = ServerDict.FirstOrDefault(q => q.Value.RingItemAdorn == "1" || q.Value.RingItemAdorn == "2").Key;
                     if (!ServerDict.ContainsKey(client_p.Key))
                     {

                         ServerDict.Add(client_p.Key, client_p.Value);
                         if (firstKey != 0)
                         {
                             //AscensionServer._Log.Info("<>" + firstKey);
                             var tempDict = ServerDict;
                             var indexOne = ServerDict.ToList().FindIndex(s => s.Key == client_p.Key);
                             var indexTwo = ServerDict.ToList().FindIndex(s => s.Key == firstKey);
                             //AscensionServer._Log.Info("<>" + indexOne);//-1
                             //AscensionServer._Log.Info("<>" + indexTwo);
                             for (int i = 0; i < ServerDict.Count; i++)
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
                                 posDict.Add(ServerDict.ToList()[numb].Key, ServerDict.ToList()[numb].Value);
                             }
                         }
                     }
                     else
                     {
                         var severValue = ServerDict[client_p.Key];
                         if (client_p.Value.RingItemCount > 0)
                             severValue.RingItemCount += client_p.Value.RingItemCount;
                         if (severValue.RingItemAdorn != client_p.Value.RingItemAdorn)
                             severValue.RingItemAdorn = client_p.Value.RingItemAdorn;
                         if (severValue.RingItemTime != client_p.Value.RingItemTime)
                             severValue.RingItemTime = client_p.Value.RingItemTime;
                     }
                     ServerDict = posDict.Count == 0 ? ServerDict : posDict;
                 */
                    #endregion

                    NHibernateQuerier.Update(new Ring() { ID = ringServerArray.ID, RingId = ringServerArray.RingId, RingItems = Utility.Json.ToJson(ServerDict), RingMagicDictServer = ringServerArray.RingMagicDictServer , RingAdorn =Utility.Json.ToJson(ServerDictAdorn) });
                    operationResponse.Parameters = subResponseParameters;
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID, nHCriteriaRingID);
            return operationResponse;
        }
    }
}


