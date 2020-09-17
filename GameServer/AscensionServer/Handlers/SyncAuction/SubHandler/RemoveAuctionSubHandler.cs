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
    public class RemoveAuctionSubHandler : SyncAuctionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);

            string[] allGoodsGuidArr = RedisHelper.Hash.HashKeysAsync("AuctionGoodsData").Result;

            for (int i = 0; i < allGoodsGuidArr.Length; i++)
            {
                await RedisHelper.KeyDeleteAsync("AuctionGoods_" + allGoodsGuidArr[i]);
            }
            await RedisHelper.KeyDeleteAsync("AuctionGoodsData");
            await RedisHelper.KeyDeleteAsync("AuctionIndex");
            await RedisHelper.KeyDeleteAsync("RoleAuctionItems");

            Owner.OpResponse.Parameters = Owner.ResponseData;
            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
