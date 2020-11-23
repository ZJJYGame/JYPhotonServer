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
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
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
            Utility.Debug.LogInfo("开始处理购买拍卖品的请求");
            byte buyCode= await BuyAuctionGoods(auctionGoodsDTO, roleId);
            Utility.Debug.LogInfo("购买请求成功完成，开始获取所有商品列表");
            List<AuctionGoodsDTO> allAuctionGoodsDTOList = await GetAuctionGoodsList(auctionGoodsDTO.GlobalID);
            Utility.Debug.LogInfo("抽取目标商品列表");
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
            Utility.Debug.LogInfo("抽取目标商品列表成功！！！！");
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(targetAuctionGoodsDTOList));
            subResponseParametersDict.Add((byte)ParameterCode.Auction, startIndex);
            subResponseParametersDict.Add((byte)ParameterCode.PutAwayAuctionGoods, allAuctionGoodsDTOList.Count);
            subResponseParametersDict.Add((byte)ParameterCode.RoleAuctionItems, buyCode);
            subResponseParametersDict.Add((byte)ParameterCode.SoldOutAuctionGoods, (byte)SyncAuctionType.BuyAuctionGoods);

            opData.DataMessage = subResponseParametersDict;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
            Utility.Debug.LogInfo("发送购买信息给客户端");
        }
       
    }
}
