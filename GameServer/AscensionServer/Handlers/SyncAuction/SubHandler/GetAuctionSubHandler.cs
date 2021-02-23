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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {

            var dict = operationRequest.Parameters;
            string dictJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Auction));
            Dictionary<byte, object> tempDict = Utility.Json.ToObject<Dictionary<byte, object>>(dictJson);

            int roleId =Int16.Parse(tempDict[(byte)ParameterCode.RoleAuctionItems].ToString());
            Utility.Debug.LogInfo("请求拍卖行数据RoleID=>"+roleId);
            SyncAuctionType syncAuctionType = (SyncAuctionType)Byte.Parse(tempDict[(byte)ParameterCode.SoldOutAuctionGoods].ToString());
            Utility.Debug.LogInfo("请求拍卖行数据RoleID~~~~~~=>"+ syncAuctionType.ToString());

            switch (syncAuctionType)
            {
                case SyncAuctionType.GetAuctionGoods:
                    int auctionGoodsId = Int16.Parse(tempDict[(byte)ParameterCode.AddAuctionGoods].ToString());
                    int startIndex = Int16.Parse(tempDict[(byte)ParameterCode.Auction].ToString());
                    int count = Int16.Parse(tempDict[(byte)ParameterCode.PutAwayAuctionGoods].ToString());
                    GameEntry.AuctionManager.ServerAuctionGoodsGet(auctionGoodsId, startIndex, count, roleId);
                    break;
                case SyncAuctionType.PutAwayAuctionGoods:
                    Utility.Debug.LogInfo("进入上架拍卖品事件");
                    AuctionGoodsDTO putAwayAuctionGoods = Utility.Json.ToObject<AuctionGoodsDTO>(tempDict[(byte)ParameterCode.AddAuctionGoods].ToString());
                    int putAwayItemId = Convert.ToInt32(tempDict[(byte)ParameterCode.Auction].ToString());
                    GameEntry.AuctionManager.SeverAuctionGoodsPutAway(putAwayAuctionGoods, putAwayItemId,roleId);
                    break;
                case SyncAuctionType.SoldOutAuctionGoods:
                    break;
                case SyncAuctionType.BuyAuctionGoods:
                    Utility.Debug.LogInfo("进入购买事件");
                    AuctionGoodsDTO auctionGoodsDTO = Utility.Json.ToObject<AuctionGoodsDTO>(tempDict[(byte)ParameterCode.AddAuctionGoods].ToString());
                    startIndex = Convert.ToInt32(tempDict[(byte)ParameterCode.Auction].ToString());
                    count = Convert.ToInt32(tempDict[(byte)ParameterCode.PutAwayAuctionGoods].ToString());
                    GameEntry.AuctionManager.SeverAuctionGoodsBuy(auctionGoodsDTO, startIndex, count, roleId);
                    break;
            }
            Dictionary<string, string> resultDict = new Dictionary<string, string>();
;
            SetResponseParamters(() =>
            {
                Utility.Debug.LogInfo("发送数据");
                string resultJson = Utility.Json.ToJson(resultDict);
                subResponseParameters.Add((byte)ParameterCode.Auction, resultJson);
                operationResponse.ReturnCode = (short)ReturnCode.Success;
                Utility.Debug.LogInfo("发送数据完成"+ resultJson);
            });
            return operationResponse;
        }
    }
}


