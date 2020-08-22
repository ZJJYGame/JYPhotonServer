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
    public class UpdateAuctionSubHandler : SyncAuctionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string buyAuctionGoodsJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AddAuctionGoods)); ;
            var buyAuctionGoodsObj = Utility.Json.ToObject<AuctionGoodsDTO>(buyAuctionGoodsJson);

            var auctionGoodsJson = RedisHelper.String.StringGetAsync("AuctionGoods" + buyAuctionGoodsObj.GUID).Result;
            if (string.IsNullOrEmpty(auctionGoodsJson))//没有数据。失败
            {

            }
            else//获得数据，继续判断
            {

            }
        }
    }
}
