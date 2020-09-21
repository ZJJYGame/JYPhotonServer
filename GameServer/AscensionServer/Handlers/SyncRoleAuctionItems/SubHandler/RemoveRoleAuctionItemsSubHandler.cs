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
    public class RemoveRoleAuctionItemsSubHandler : SyncRoleAuctionItemsSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            bool isSuccess = true;

            ResetResponseData(operationRequest);

            var dict = ParseSubDict(operationRequest);
            string removeRoleAuctionItemJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));
            RoleAuctionItem removeRoleAuctionItem = Utility.Json.ToObject<RoleAuctionItem>(removeRoleAuctionItemJson);

            await RedisHelper.KeyDeleteAsync("AuctionGoods_" + removeRoleAuctionItem.AuctionGoods.GUID);
            

            if (RedisHelper.Hash.HashExistAsync("AuctionGoodsData", removeRoleAuctionItem.AuctionGoods.GUID).Result)
            {
                Utility.Debug.LogInfo("移除数据表");
                await RedisHelper.Hash.HashDeleteAsync("AuctionGoodsData", removeRoleAuctionItem.AuctionGoods.GUID);
            }
            else
            {
                isSuccess = false;
            }

            if (RedisHelper.Hash.HashExistAsync("AuctionIndex", removeRoleAuctionItem.AuctionGoods.GlobalID.ToString()).Result&&isSuccess)
            {
                Utility.Debug.LogInfo("移除索引表");
                List<AuctionGoodsIndex> tempAuctionGoodsIndexList = RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", removeRoleAuctionItem.AuctionGoods.GlobalID.ToString()).Result;
                AuctionGoodsIndex tempAuctionGoodsIndex = tempAuctionGoodsIndexList.Find(p => p.RedisKey == removeRoleAuctionItem.AuctionGoods.GUID);
                if (tempAuctionGoodsIndex != null)
                {
                    tempAuctionGoodsIndexList.Remove(tempAuctionGoodsIndex);
                    await RedisHelper.Hash.HashSetAsync("AuctionIndex", removeRoleAuctionItem.AuctionGoods.GlobalID.ToString(), tempAuctionGoodsIndexList);
                }
            }
            else
            {
                isSuccess = false;
            }

            if (RedisHelper.Hash.HashExistAsync("RoleAuctionItems", removeRoleAuctionItem.AuctionGoods.RoleID.ToString()).Result&&isSuccess)
            {
                Utility.Debug.LogInfo("移除个人表");
                List<string> tempGuidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", removeRoleAuctionItem.AuctionGoods.RoleID.ToString()).Result;
                if (tempGuidList.Contains(removeRoleAuctionItem.AuctionGoods.GUID))
                {
                    tempGuidList.Remove(removeRoleAuctionItem.AuctionGoods.GUID);
                    await RedisHelper.Hash.HashSetAsync("RoleAuctionItems", removeRoleAuctionItem.AuctionGoods.RoleID.ToString(), tempGuidList);
                }
            }
            else
            {
                isSuccess = false;
            }

            //获取个人拍卖品列表
            List<RoleAuctionItem> roleAuctionItemList = new List<RoleAuctionItem>();
            if (RedisHelper.Hash.HashExistAsync("RoleAuctionItems", removeRoleAuctionItem.AuctionGoods.RoleID.ToString()).Result)
            {
                List<string> tempGuidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", removeRoleAuctionItem.AuctionGoods.RoleID.ToString()).Result;
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
                Utility.Debug.LogInfo("移除成功"+ Utility.Json.ToJson(removeRoleAuctionItem));
                Utility.Debug.LogInfo("移除成功"+ Utility.Json.ToJson(roleAuctionItemList));
                Owner.ResponseData.Add((byte)ParameterCode.RoleAuctionItems, Utility.Json.ToJson (removeRoleAuctionItem));
                Owner.ResponseData.Add((byte)ParameterCode.Auction, Utility.Json.ToJson(roleAuctionItemList));
                Owner.OpResponseData.Parameters = Owner.ResponseData;
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
              
            }
            else
            {
                Utility.Debug.LogInfo("移除失败");
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            Utility.Debug.LogInfo("移除事件结束");

        }

        
    }
}
