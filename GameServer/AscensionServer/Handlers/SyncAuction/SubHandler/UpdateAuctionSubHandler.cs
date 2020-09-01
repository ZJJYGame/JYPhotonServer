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

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            Utility.Debug.LogInfo("进入更新拍卖品事件");
            var dict = ParseSubDict(operationRequest);
            string buyAuctionGoodsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods));
            Dictionary<string, string> dataDict = Utility.Json.ToObject<Dictionary<string, string>>(buyAuctionGoodsJson);
            var buyAuctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(dataDict["Data"]);
            int startIndex = Convert.ToInt32(dataDict["Start"]);
            int count = Convert.ToInt32(dataDict["Count"]);
            int allGoodsCount=0;

            var auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods" + buyAuctionGoodsObj.GUID).Result;
            Utility.Debug.LogInfo("AuctionGoods" + buyAuctionGoodsObj.GUID);
            Utility.Debug.LogInfo(auctionGoodsJson);

            AuctionGoodsDTO resultAuctionGoodsDTO= new AuctionGoodsDTO();

            if (string.IsNullOrEmpty(auctionGoodsJson))//没有数据。失败
            {
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo("找不到该商品");
                    List<AuctionGoodsDTO> tempAuctionGoodsDTOs = GetReturnGoodsList(buyAuctionGoodsObj.GlobalID, ref startIndex, count,ref allGoodsCount);
                    Dictionary<string, string> resultDict = new Dictionary<string, string>();
                    resultDict.Add("Data",Utility.Json.ToJson(resultAuctionGoodsDTO));
                    resultDict.Add("List", Utility.Json.ToJson(tempAuctionGoodsDTOs));
                    resultDict.Add("StartIndex",startIndex.ToString());
                    resultDict.Add("Count",allGoodsCount.ToString());
                    SubDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(resultDict));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    Utility.Debug.LogInfo("找不到该商品");
                });
            }
            else//获得数据，继续判断
            {
                var auctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson);
                if (buyAuctionGoodsObj.Count <= auctionGoodsObj.Count)//购买数量足够
                {
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
                        RedisHelper.KeyDeleteAsync("AuctionGoods" + auctionGoodsObj.GUID);

                        List<AuctionGoodsIndex> tempAuctionGoodIndexs = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result;
                        AuctionGoodsIndex auctionGoodsIndex = tempAuctionGoodIndexs.Find((p) => p.RedisKey == auctionGoodsObj.GUID);
                        tempAuctionGoodIndexs.Remove(auctionGoodsIndex);
                        if (tempAuctionGoodIndexs.Count != 0)//购买后当前种物品索引数量 不为0
                        {
                            RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoodIndexs);
                        }
                        else
                        {
                            RedisHelper.Hash.HashDeleteAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString());
                        }
                    }
                    else
                    {
                        RedisHelper.String.StringSetAsync("AuctionGoods" + auctionGoodsObj.GUID, Utility.Json.ToJson(auctionGoodsObj));
                    }
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo("购买物品成功");
                        List<AuctionGoodsDTO> tempAuctionGoodsDTOs = GetReturnGoodsList(buyAuctionGoodsObj.GlobalID, ref startIndex, count, ref allGoodsCount);
                        Dictionary<string, string> resultDict = new Dictionary<string, string>();
                        resultDict.Add("Data", Utility.Json.ToJson(resultAuctionGoodsDTO));
                        resultDict.Add("List", Utility.Json.ToJson(tempAuctionGoodsDTOs));
                        resultDict.Add("StartIndex", startIndex.ToString());
                        resultDict.Add("Count", allGoodsCount.ToString());
                        SubDict.Add((byte)ParameterCode.AddAuctionGoods, resultDict);
                        Utility.Debug.LogInfo(Utility.Json.ToJson(resultDict));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else//购买数量不足
                {
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo("物品数量不足");
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
            if (!RedisHelper.Hash.HashExistAsync("AuctionIndex", id.ToString()).Result)
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
                string auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods" + result[i].RedisKey).Result;
                auctionGoodsDTOList.Add(Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson));
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
