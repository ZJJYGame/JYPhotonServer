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
    public class AddRoleAuctionAttentionSubHandler : SyncRoleAuctionAttentionSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            Utility.Debug.LogInfo("进入关注事件");
            ResetResponseData(operationRequest);
            var dict = ParseSubParameters(operationRequest);
            string guid= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));
            int roleID= Convert.ToInt32(Utility.GetValue(dict, (byte)ParameterCode.Auction));
            List<string> guidList;
            if (RedisHelper.Hash.HashExistAsync("RoleAuctionAttention",roleID.ToString()).Result)
            {
                guidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionAttention", roleID.ToString()).Result;
                if (!guidList.Contains(guid))
                {
                    guidList.Add(guid);
                    RedisHelper.Hash.HashSet("RoleAuctionAttention", roleID.ToString(), guidList);
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                }
                else
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                }
            }
            else
            {
                guidList = new List<string>();
                guidList.Add(guid);
                RedisHelper.Hash.HashSet("RoleAuctionAttention", roleID.ToString(), guidList);
            }
            operationResponse.Parameters = subResponseParameters;
            if (operationResponse.ReturnCode == (short)ReturnCode.Success)
            {
                List<AuctionGoodsDTO> resultList = new List<AuctionGoodsDTO>();
                for (int i = 0; i < guidList.Count; i++)
                {
                    if (RedisHelper.Hash.HashExistAsync("AuctionGoodsData", guidList[i]).Result)
                    {
                        resultList.Add(RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", guidList[i]).Result);
                    }
                }
            subResponseParameters.Add((byte)ParameterCode.RoleAuctionItems, Utility.Json.ToJson(resultList));
            }
            return operationResponse;
        }
    }
}
