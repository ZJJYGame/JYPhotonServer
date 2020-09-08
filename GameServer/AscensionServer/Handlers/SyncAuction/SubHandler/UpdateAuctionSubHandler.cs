using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
    public class UpdateAuctionSubHandler : SyncAuctionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            Utility.Debug.LogInfo("进入更新拍卖品事件");
            var dict = ParseSubDict(operationRequest);
            string buyAuctionGoodsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods));
            Dictionary<string, string> dataDict = Utility.Json.ToObject<Dictionary<string, string>>(buyAuctionGoodsJson);
            var buyAuctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(dataDict["Data"]);
            int startIndex = Convert.ToInt32(dataDict["Start"]);
            int count = Convert.ToInt32(dataDict["Count"]);
            int roleID = Convert.ToInt32(dataDict["RoleID"]);
            int allGoodsCount=0;

            string auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods_" + buyAuctionGoodsObj.GUID).Result;
            Utility.Debug.LogInfo(roleID+"AuctionGoods_" + buyAuctionGoodsObj.GUID);
            Utility.Debug.LogInfo(roleID + auctionGoodsJson);

            AuctionGoodsDTO resultAuctionGoodsDTO= new AuctionGoodsDTO();

            if (auctionGoodsJson == null)//没有数据。失败
            {
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo(roleID + "找不到该商品");
                    List<AuctionGoodsDTO> tempAuctionGoodsDTOs = GetReturnGoodsList(buyAuctionGoodsObj.GlobalID, ref startIndex, count,ref allGoodsCount);
                    Dictionary<string, string> resultDict = new Dictionary<string, string>();
                    resultDict.Add("Data",Utility.Json.ToJson(resultAuctionGoodsDTO));
                    resultDict.Add("List", Utility.Json.ToJson(tempAuctionGoodsDTOs));
                    resultDict.Add("StartIndex",startIndex.ToString());
                    resultDict.Add("Count",allGoodsCount.ToString());
                    SubDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(resultDict));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Empty;
                    Utility.Debug.LogInfo(roleID+Utility.Json.ToJson(resultDict));
                    Utility.Debug.LogInfo(roleID + "找不到该商品");
                });
            }
            else//获得数据，继续判断
            {
                Utility.Debug.LogInfo(roleID + "找到该商品");
                var auctionGoodsObj = RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", buyAuctionGoodsObj.GUID).Result;
                if (buyAuctionGoodsObj.Count <= auctionGoodsObj.Count)//购买数量足够
                {
                    Utility.Debug.LogInfo(roleID + "购买数量足够");
                    resultAuctionGoodsDTO = new AuctionGoodsDTO()
                    {
                        GUID = auctionGoodsObj.GUID,
                        RoleID = auctionGoodsObj.RoleID,
                        GlobalID = auctionGoodsObj.GlobalID,
                        Price = auctionGoodsObj.Price,
                        ItemData = auctionGoodsObj.ItemData,
                        Count= buyAuctionGoodsObj.Count
                    };
                    auctionGoodsObj.Count -= buyAuctionGoodsObj.Count;
                    if (auctionGoodsObj.Count == 0)//买完了
                    {
                        Utility.Debug.LogInfo(roleID + "买完了");
                        await RedisHelper.KeyDeleteAsync("AuctionGoods_" + auctionGoodsObj.GUID);

                        await RedisHelper.Hash.HashDeleteAsync("AuctionGoodsData", buyAuctionGoodsObj.GUID);

                        List<string> roleAuctionGuidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", auctionGoodsObj.RoleID.ToString()).Result;
                        string guidStr = roleAuctionGuidList.Find((p) => p == auctionGoodsObj.GUID);
                        roleAuctionGuidList.Remove(guidStr);
                        if (roleAuctionGuidList.Count != 0)
                        {
                            await RedisHelper.Hash.HashSetAsync("RoleAuctionItems", auctionGoodsObj.RoleID.ToString(), roleAuctionGuidList);
                        }
                        else
                        {
                            await RedisHelper.Hash.HashDeleteAsync("RoleAuctionItems", auctionGoodsObj.RoleID.ToString());
                        }

                        List<AuctionGoodsIndex> tempAuctionGoodIndexs = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result;
                        AuctionGoodsIndex auctionGoodsIndex = tempAuctionGoodIndexs.Find((p) => p.RedisKey == auctionGoodsObj.GUID);
                        tempAuctionGoodIndexs.Remove(auctionGoodsIndex);
                        if (tempAuctionGoodIndexs.Count != 0)//购买后当前种物品索引数量 不为0
                        {
                            await RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoodIndexs);
                        }
                        else
                        {
                            await RedisHelper.Hash.HashDeleteAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString());
                        }
                    }
                    else
                    {
                        await RedisHelper.Hash.HashSetAsync("AuctionGoodsData", buyAuctionGoodsObj.GUID, auctionGoodsObj);
                    }
                    Utility.Debug.LogInfo(roleID + "主备发送成功信息");
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo(roleID + "购买物品成功");
                        List<AuctionGoodsDTO> tempAuctionGoodsDTOs = GetReturnGoodsList(buyAuctionGoodsObj.GlobalID, ref startIndex, count, ref allGoodsCount);
                        Dictionary<string, string> resultDict = new Dictionary<string, string>();
                        resultDict.Add("Data", Utility.Json.ToJson(resultAuctionGoodsDTO));
                        resultDict.Add("List", Utility.Json.ToJson(tempAuctionGoodsDTOs));
                        resultDict.Add("StartIndex", startIndex.ToString());
                        resultDict.Add("Count", allGoodsCount.ToString());
                        SubDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(resultDict));
                        Utility.Debug.LogInfo(roleID + Utility.Json.ToJson(resultDict));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else//购买数量不足
                {
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo(roleID + "物品数量不足");
                        List<AuctionGoodsDTO> tempAuctionGoodsDTOs = GetReturnGoodsList(buyAuctionGoodsObj.GlobalID, ref startIndex, count, ref allGoodsCount);
                        Dictionary<string, string> resultDict = new Dictionary<string, string>();
                        resultDict.Add("Data", Utility.Json.ToJson(resultAuctionGoodsDTO));
                        resultDict.Add("List", Utility.Json.ToJson(tempAuctionGoodsDTOs));
                        resultDict.Add("StartIndex", startIndex.ToString());
                        resultDict.Add("Count", allGoodsCount.ToString());
                        SubDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(resultDict));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
              
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }

         List<AuctionGoodsDTO> GetReturnGoodsList(int id,ref int startIndex,int count,ref int allGoodsCount)
        {
            List<AuctionGoodsDTO> auctionGoodsDTOList = new List<AuctionGoodsDTO>();
            if (! RedisHelper.Hash.HashExistAsync("AuctionIndex", id.ToString()).Result)
            {
                Utility.Debug.LogInfo("当前种类商品不存在");
                return auctionGoodsDTOList;
            }
               
            //todo 表不存在，直接返回
            List<AuctionGoodsIndex> result = RedisHelper.Hash.HashGet<List<AuctionGoodsIndex>>("AuctionIndex", id.ToString());
            allGoodsCount = result.Count;
            if (result.Count == 0)
            {
                return auctionGoodsDTOList;
            }
            if (startIndex >= result.Count)
            {
                startIndex = ((result.Count-1) / count)*count;
            }
            for (int i = 0; i < result.Count; i++)
            {
                auctionGoodsDTOList.Add(RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", result[i].RedisKey).Result);
            }
            if (startIndex + count <= result.Count)
            {
                auctionGoodsDTOList = auctionGoodsDTOList.GetRange(startIndex, count);
            }
            else
            {
                auctionGoodsDTOList = auctionGoodsDTOList.GetRange(startIndex, result.Count - startIndex);
            }
            return auctionGoodsDTOList;
        }

    }
}
