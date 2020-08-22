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
            string auctionGoodsJson= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods));
            var auctionGoodsObj = Utility.Json.ToObject<AuctionGoods>(auctionGoodsJson);
            string guid= Guid.NewGuid().ToString("N");
            auctionGoodsObj.GUID = guid;

            string redisKey = "AuctionGoods" + guid;
            RedisHelper.String.StringSetAsync(redisKey, Utility.Json.ToJson(auctionGoodsObj));

            if (RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result)
            {
                List<AuctionGoodsIndex> tempAuctionGoods = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result;
                tempAuctionGoods.Add(new AuctionGoodsIndex()
                {
                    RedisKey = auctionGoodsObj.GUID,
                    Price = auctionGoodsObj.Price
                });
                tempAuctionGoods.Sort();
                RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoods);
            }
            else
            {

            }
        }
    }
}
