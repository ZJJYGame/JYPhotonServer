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
    public class GetRoleAuctionItemsSubHandler : SyncRoleAuctionItemsSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            int roleID = Convert.ToInt32(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));
            List<RoleAuctionItem> roleAuctionItemList = new List<RoleAuctionItem>();
            if (RedisHelper.Hash.HashExistAsync("RoleAuctionItems", roleID.ToString()).Result)
            {
                List<string> tempGuidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", roleID.ToString()).Result;
                for (int i = 0; i < tempGuidList.Count; i++)
                {
                    if (RedisHelper.String.StringGetAsync("AuctionGoods_" + tempGuidList[i]).Result != null)
                    {
                        roleAuctionItemList.Add(new RoleAuctionItem()
                        {
                            AuctionGoods=RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", "AuctionGoods_" + tempGuidList[i]).Result,
                            IsPutAway=true
                        });
                    }
                    else
                    {
                        roleAuctionItemList.Add(new RoleAuctionItem()
                        {
                            AuctionGoods = RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", "AuctionGoods_" + tempGuidList[i]).Result,
                            IsPutAway = false
                        });
                    }
                }
            }
            SetResponseData(() =>
            {
                Utility.Debug.LogInfo(Utility.Json.ToJson(roleAuctionItemList));
                SubDict.Add((byte)ParameterCode.RoleAuctionItems, Utility.Json.ToJson(roleAuctionItemList));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
