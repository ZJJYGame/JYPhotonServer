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
    public class AddRoleAuctionItemSubHandler : SyncRoleAuctionItemsSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            bool isSuccess = true;

            ResetResponseData(operationRequest);

            var dict = ParseSubDict(operationRequest);
            string auctionGoodsDTOJson= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));
            AuctionGoodsDTO auctionGoodsDTO = Utility.Json.ToObject<AuctionGoodsDTO>(auctionGoodsDTOJson);

            
            if (RedisHelper.Hash.HashExistAsync("AuctionGoodsData", auctionGoodsDTO.GUID).Result&&isSuccess)
            {
                Utility.Debug.LogInfo("重新设置数据表");
                await RedisHelper.Hash.HashSetAsync("AuctionGoodsData", auctionGoodsDTO.GUID.ToString(), auctionGoodsDTO);
            }
            else
            {
                isSuccess = false;
            }
            if (RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsDTO.GlobalID.ToString()).Result&&isSuccess)
            {
                Utility.Debug.LogInfo("重新设置索引表");
                List<AuctionGoodsIndex> auctionGoodsIndexList = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsDTO.GlobalID.ToString()).Result;
                AuctionGoodsIndex tempAuctionGoodsIndex = auctionGoodsIndexList.Find(p => p.RedisKey == auctionGoodsDTO.GUID);
                tempAuctionGoodsIndex.Price = auctionGoodsDTO.Price;
                auctionGoodsIndexList.Sort();
                RedisHelper.Hash.HashSet("AuctionIndex", auctionGoodsDTO.GlobalID.ToString(), auctionGoodsIndexList);
            }
            else
            {
                isSuccess = false;
            }
            if (isSuccess)
            {
                Utility.Debug.LogInfo("重新添加标记索引");
                await RedisHelper.String.StringSetAsync("AuctionGoods_" + auctionGoodsDTO.GUID, "");
            }

            //获取个人拍卖品列表
            List<RoleAuctionItem> roleAuctionItemList = new List<RoleAuctionItem>();
            if (RedisHelper.Hash.HashExistAsync("RoleAuctionItems", auctionGoodsDTO.RoleID.ToString()).Result)
            {
                List<string> tempGuidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", auctionGoodsDTO.RoleID.ToString()).Result;
                for (int i = 0; i < tempGuidList.Count; i++)
                {
                    if (RedisHelper.String.StringGetAsync("AuctionGoods_" + tempGuidList[i]).Result != null)
                    {
                        roleAuctionItemList.Add(new RoleAuctionItem()
                        {
                            AuctionGoods = RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", tempGuidList[i]).Result,
                            IsPutAway = true
                        });
                    }
                    else
                    {
                        roleAuctionItemList.Add(new RoleAuctionItem()
                        {
                            AuctionGoods = RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", tempGuidList[i]).Result,
                            IsPutAway = false
                        });
                    }
                }
            }

            if (isSuccess)
            {
                Utility.Debug.LogInfo("重新上架成功");
                Owner.ResponseData.Add((byte)ParameterCode.RoleAuctionItems, Utility.Json.ToJson(roleAuctionItemList));
                Owner.ResponseData.Add((byte)ParameterCode.Auction, Utility.Json.ToJson(auctionGoodsDTO));
                Owner.OpResponseData.Parameters = Owner.ResponseData;
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                Utility.Debug.LogInfo("重新上架失败");
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            Utility.Debug.LogInfo("重新上架事件结束");
        }
    }
}
