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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string auctionGoodsJson= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods));;
            var auctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson);
            string guid= Guid.NewGuid().ToString("N");
            auctionGoodsObj.GUID = guid;

            string redisKey = "AuctionGoods_" + guid;
            RedisHelper.String.StringSet(redisKey, "");

            //加入索引表
            if (RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result)
            {
                List<AuctionGoodsIndex> tempAuctionGoods = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString()).Result;
                tempAuctionGoods.Add(new AuctionGoodsIndex()
                {
                    RedisKey = auctionGoodsObj.GUID,
                    Price = auctionGoodsObj.Price
                });
                Utility.Debug.LogInfo("存在key");
                Utility.Debug.LogInfo(Utility.Json.ToJson(tempAuctionGoods));
                tempAuctionGoods.Sort();
                Utility.Debug.LogInfo(Utility.Json.ToJson(tempAuctionGoods));

                RedisHelper.Hash.HashSet("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoods);
            }
            else
            {
                Utility.Debug.LogInfo("不存在key");
                List<AuctionGoodsIndex> tempAuctionGoods = new List<AuctionGoodsIndex>();
                tempAuctionGoods.Add(new AuctionGoodsIndex()
                {
                    RedisKey = auctionGoodsObj.GUID,
                    Price = auctionGoodsObj.Price
                });
                RedisHelper.Hash.HashSet("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoods);
            }
            //加入数据表
            if (!RedisHelper.Hash.HashExistAsync("AuctionGoodsData", auctionGoodsObj.GUID.ToString()).Result)
                RedisHelper.Hash.HashSet("AuctionGoodsData", auctionGoodsObj.GUID.ToString(), auctionGoodsObj);

            List<string> roleAuctionItemList = new List<string>();
            //更新玩家个人拍卖表数据
            if(RedisHelper.Hash.HashExistAsync("RoleAuctionItems", auctionGoodsObj.RoleID.ToString()).Result)
            {
                roleAuctionItemList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", auctionGoodsObj.RoleID.ToString()).Result;
                roleAuctionItemList.Add(auctionGoodsObj.GUID);
                Utility.Debug.LogInfo("玩家拍卖列表存在key");
                RedisHelper.Hash.HashSet("RoleAuctionItems", auctionGoodsObj.RoleID.ToString(), roleAuctionItemList);
            }
            else
            {
                Utility.Debug.LogInfo("玩家拍卖列表不存在key!!!!");
                roleAuctionItemList.Add(auctionGoodsObj.GUID);
                RedisHelper.Hash.HashSet("RoleAuctionItems", auctionGoodsObj.RoleID.ToString(), roleAuctionItemList);
            }
            subResponseParameters.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(auctionGoodsObj));
            operationResponse.Parameters = subResponseParameters;
            operationResponse.ReturnCode = (short)ReturnCode.Success;
            return operationResponse;
        }
    }
}
