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
        public async void ServerAuctionGoodsGet(int auctionGoodsID, int startIndex, int count)
        {
            List<AuctionGoodsDTO> allAuctionGoodsDTOList = await GetAuctionGoodsList(auctionGoodsID);
            List<AuctionGoodsDTO> targetAuctionGoodsDTOList = await GetTargetAuctionGoodList(allAuctionGoodsDTOList, startIndex, count);
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAuction;

            Dictionary<string, string> resultDict = new Dictionary<string, string>();
            resultDict.Add("Data", Utility.Json.ToJson(targetAuctionGoodsDTOList));
            resultDict.Add("Count", allAuctionGoodsDTOList.Count.ToString());
            resultDict.Add("Index", startIndex.ToString());

            Dictionary<byte, object> subResponseParametersDict = new Dictionary<byte, object>();
            subResponseParametersDict.Add((byte)ParameterCode.AddAuctionGoods, Utility.Json.ToJson(resultDict));

            opData.DataMessage = subResponseParametersDict;
        } 

       
    }
}
