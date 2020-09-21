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
    public class GetRoleAuctionAttentionSubHandler : SyncRoleAuctionAttentionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var dict = ParseSubDict(operationRequest);
            int roleID= Convert.ToInt32(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));

            List<AuctionGoodsDTO> resultAuctionGoodsList = new List<AuctionGoodsDTO>();

            if(RedisHelper.Hash.HashExistAsync("RoleAuctionAttention", roleID.ToString()).Result)
            {
                List<string> guidList= RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionAttention", roleID.ToString()).Result;
                for (int i = guidList.Count-1; i >=0; i--)
                {
                    if (RedisHelper.String.StringGetAsync("AuctionGoods_" + guidList[i]).Result == null)
                    {
                        guidList.RemoveAt(i);
                    }
                }
                await RedisHelper.Hash.HashSetAsync("RoleAuctionAttention", roleID.ToString(), guidList);
                for (int i = 0; i < guidList.Count; i++)
                {
                    if (RedisHelper.Hash.HashExistAsync("AuctionGoodsData", guidList[i]).Result)
                    {
                        AuctionGoodsDTO tempAuctionGoodsDTO = RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", guidList[i]).Result;
                        resultAuctionGoodsList.Add(tempAuctionGoodsDTO);
                    }
                }
            }
            Owner.ResponseData.Add((byte)ParameterCode.RoleAuctionItems, Utility.Json.ToJson(resultAuctionGoodsList));
            Owner.OpResponse.Parameters = Owner.ResponseData;
            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
