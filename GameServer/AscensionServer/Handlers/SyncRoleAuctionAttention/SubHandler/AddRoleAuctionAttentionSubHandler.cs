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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var dict = ParseSubDict(operationRequest);
            string guid= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAuctionItems));
            int roleID= Convert.ToInt32(Utility.GetValue(dict, (byte)ParameterCode.Auction));

            if (RedisHelper.Hash.HashExistAsync("RoleAuctionAttention",roleID.ToString()).Result)
            {
                List<string> guidList = RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionAttention", roleID.ToString()).Result;
                guidList.Add(guid);
                await RedisHelper.Hash.HashSetAsync("RoleAuctionAttention", roleID.ToString(),guidList);
            }
            else
            {
                List<string> guidList = new List<string>();
                guidList.Add(guid);
                await RedisHelper.Hash.HashSetAsync("RoleAuctionAttention", roleID.ToString(), guidList);
            }

            Owner.OpResponse.Parameters = Owner.ResponseData;
            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
