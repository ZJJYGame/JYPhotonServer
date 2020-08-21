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
            int auctionGoodsID = Convert.ToInt32(Utility.GetValue(dict, (byte)ParameterCode.Auction));
            AscensionServer._Log.Info("收到的拍卖行"+ auctionGoodsID);

            List<AuctionGoodsIndex> result=null;
            var isHasValue = RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsID.ToString()).Result;
            if (!isHasValue)
            {
                AscensionServer._Log.Info("redis拍卖行索引表不存在该ID");
            }
            else
            {
                AscensionServer._Log.Info("redis拍卖行索引表存在该ID");
                result = RedisHelper.Hash.HashGet<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsID.ToString());
            }
            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.Auction, Utility.Json.ToJson(result));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
