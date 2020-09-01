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
    public class GetAuctionSubHandler : SyncAuctionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string dictJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Auction));
            Dictionary<string, int> tempDict = Utility.Json.ToObject<Dictionary<string, int>>(dictJson);
            int auctionGoodsID = tempDict["ID"];
            int startIndex = tempDict["Start"];
            int count = tempDict["Count"];
            int goodsCount=0;

            Utility.Debug.LogInfo("收到的拍卖行"+ auctionGoodsID+"起始"+startIndex+"数量"+count);

            List<AuctionGoodsIndex> result=null;
            List<AuctionGoodsDTO> auctionGoodsDTOList = new List<AuctionGoodsDTO>();
            List<AuctionGoodsDTO> resultAuctionGoodsDTOList = new List<AuctionGoodsDTO>();
            var isHasValue = RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsID.ToString()).Result;
            if (!isHasValue)
            {
                Utility.Debug.LogInfo("redis拍卖行索引表不存在该ID");
            }
            else
            {
                Utility.Debug.LogInfo("redis拍卖行索引表存在该ID");
                result = RedisHelper.Hash.HashGet<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsID.ToString());
                goodsCount = result.Count;
                if (startIndex >= result.Count)
                    return;
                for (int i = 0; i < result.Count; i++)
                {
                    string auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods"+result[i].RedisKey).Result;
                    auctionGoodsDTOList.Add(Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsJson));
                }

                if (startIndex+count <= goodsCount)
                {
                    resultAuctionGoodsDTOList = auctionGoodsDTOList.GetRange(startIndex, count);
                }
                else
                {
                    resultAuctionGoodsDTOList = auctionGoodsDTOList.GetRange(startIndex, goodsCount-startIndex);
                }
                //todo发送数据更改
            }
            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            resultDict.Add("Data", Utility.Json.ToJson(resultAuctionGoodsDTOList));
            resultDict.Add("Count", goodsCount.ToString());
            resultDict.Add("Index", startIndex.ToString());
            SetResponseData(() =>
            {
                Utility.Debug.LogInfo("发送数据");
                SubDict.Add((byte)ParameterCode.Auction, Utility.Json.ToJson(resultDict));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                Utility.Debug.LogInfo("发送数据完成"+ Utility.Json.ToJson(resultDict));
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
