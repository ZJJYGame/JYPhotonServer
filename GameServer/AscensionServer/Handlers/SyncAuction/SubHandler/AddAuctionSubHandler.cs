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
    public class AddAuctionSubHandler : SyncAuctionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string auctionGoodsJson= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods));;
            var auctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson);
            string guid= Guid.NewGuid().ToString("N");
            auctionGoodsObj.GUID = guid;

            string redisKey = "AuctionGoods" + guid;
            RedisHelper.String.StringSetAsync(redisKey, Utility.Json.ToJson(auctionGoodsObj));

            //加入索引表
            if (RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result)
            {
                List<AuctionGoodsIndex> tempAuctionGoods = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result;
                tempAuctionGoods.Add(new AuctionGoodsIndex()
                {
                    RedisKey = auctionGoodsObj.GUID,
                    Price = auctionGoodsObj.Price
                });
                AscensionServer._Log.Info("存在key");
                AscensionServer._Log.Info(Utility.Json.ToJson(tempAuctionGoods));
                tempAuctionGoods.Sort();
                AscensionServer._Log.Info(Utility.Json.ToJson(tempAuctionGoods));

                RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoods);
            }
            else
            {
                AscensionServer._Log.Info("不存在key");
                List<AuctionGoodsIndex> tempAuctionGoods = new List<AuctionGoodsIndex>();
                tempAuctionGoods.Add(new AuctionGoodsIndex()
                {
                    RedisKey = auctionGoodsObj.GUID,
                    Price = auctionGoodsObj.Price
                });
                RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoods);
            }
           
            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.AddAuctionGoods, "");
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
