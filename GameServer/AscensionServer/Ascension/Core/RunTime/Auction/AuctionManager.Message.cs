using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using RedisDotNet;
using Cosmos;
using AscensionProtocol;
using Protocol;

namespace AscensionServer
{
    public partial class AuctionManager
    {
        public async void ServerAuctionGoodsGet(int auctionGoodsID, int startIndex, int count,int roleId)
        {
            List<AuctionGoodsDTO> allAuctionGoodsDTOList = await GetAuctionGoodsList(auctionGoodsID);
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = await GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(targetAuctionGoodsDTOList));
            subResponseParametersDict.Add((byte)ParameterCode.Auction, startIndex);
            subResponseParametersDict.Add((byte)ParameterCode.PutAwayAuctionGoods, allAuctionGoodsDTOList.Count);
            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.GetAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
            Utility.Debug.LogInfo("拍卖行数据发送给玩家");
        } 
        public async void SeverAuctionGoodsBuy(AuctionGoodsDTO auctionGoodsDTO,int startIndex,int count,int roleId)
        {

            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();

            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.BuyAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
        }
       
    }
}
