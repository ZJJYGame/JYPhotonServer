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
        /// <summary>
        /// 获取拍卖品
        /// </summary>
        public async void ServerAuctionGoodsGet(int auctionGoodsID, int startIndex, int count,int roleId)
        {
            List<AuctionGoodsDTO> allAuctionGoodsDTOList = await GetAuctionGoodsList(auctionGoodsID);
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(targetAuctionGoodsDTOList));
            subResponseParametersDict.Add((byte)ParameterCode.Auction, startIndex);
            subResponseParametersDict.Add((byte)ParameterCode.PutAwayAuctionGoods, allAuctionGoodsDTOList.Count);
            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.GetAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameEntry.RoleManager.SendMessage(roleId, opData);
        } 
        /// <summary>
        /// 购买拍卖品
        /// </summary>
        public async void SeverAuctionGoodsBuy(AuctionGoodsDTO auctionGoodsDTO,int startIndex,int count,int roleId)
        {
            byte buyCode= await BuyAuctionGoods(auctionGoodsDTO, roleId);
            List<AuctionGoodsDTO> allAuctionGoodsDTOList = await GetAuctionGoodsList(auctionGoodsDTO.GlobalID);
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(targetAuctionGoodsDTOList));
            subResponseParametersDict.Add((byte)ParameterCode.Auction, startIndex);
            subResponseParametersDict.Add((byte)ParameterCode.PutAwayAuctionGoods, allAuctionGoodsDTOList.Count);
            subResponseParametersDict.Add((byte)ParameterCode.RoleAuctionItems, buyCode);
            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.BuyAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameEntry.RoleManager.SendMessage(roleId, opData);
        }
        /// <summary>
        /// 上架拍卖品
        /// </summary>
       public async void SeverAuctionGoodsPutAway(AuctionGoodsDTO putAwayAuctionGoods,int itemId, int roleId)
        {
            Utility.Debug.LogInfo("上架拍卖品事件——1");
            await PutAwayAuctionGoods(putAwayAuctionGoods, itemId);
            Utility.Debug.LogInfo("上架拍卖品事件——2");
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;
            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.PutAwayAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameEntry.RoleManager.SendMessage(roleId, opData);
        }
    }
}


