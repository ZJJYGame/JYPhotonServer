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
            AscensionServer._Log.Info("进入更新拍卖品事件");
            var dict = ParseSubDict(operationRequest);
            string buyAuctionGoodsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods)); ;
            var buyAuctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(buyAuctionGoodsJson);

            var auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods" + buyAuctionGoodsObj.GUID).Result;
            AscensionServer._Log.Info("AuctionGoods" + buyAuctionGoodsObj.GUID);
            AscensionServer._Log.Info(auctionGoodsJson);
            if (string.IsNullOrEmpty(auctionGoodsJson))//没有数据。失败
            {
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info("找不到该商品");
                    SubDict.Add((byte)ParameterCode.AddAuctionGoods, "");
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            else//获得数据，继续判断
            {
                var auctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson);
                if (buyAuctionGoodsObj.Count <= auctionGoodsObj.Count)//购买数量足够
                {
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
                        AscensionServer._Log.Info("购买物品成功");
                        SubDict.Add((byte)ParameterCode.AddAuctionGoods, "");
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else//购买数量不足
                {
                    SetResponseData(() =>
                    {
                        AscensionServer._Log.Info("物品数量不足");
                        SubDict.Add((byte)ParameterCode.AddAuctionGoods, "");
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
            }
        }
    }
}
