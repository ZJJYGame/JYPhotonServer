using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using RedisDotNet;
using Cosmos;

namespace AscensionServer
{
    public partial class AuctionManager
    {
        /// <summary>
        /// 获取某一种物品的所有物品信息
        /// </summary>
        /// <param name="auctionGoodsID"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<AuctionGoodsDTO>> GetAuctionGoodsList(int auctionGoodsID)
        {
            int goodsCount = 0;
            List<AuctionGoodsIndex> resultIndex;
            List<AuctionGoodsIndex> newResultIndex = new List<AuctionGoodsIndex>();
            List<AuctionGoodsDTO> auctionGoodsDTOList = new List<AuctionGoodsDTO>();
            var isHasValue = await RedisHelper.Hash.HashExistAsync("AuctionIndex", auctionGoodsID.ToString());
            if (!isHasValue)
            {
                Utility.Debug.LogInfo("redis拍卖行索引表不存在该ID");
            }
            else
            {
                Utility.Debug.LogInfo("redis拍卖行索引表存在该ID");
                resultIndex = await RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsID.ToString());

                    for (int i = 0; i < resultIndex.Count; i++)
                    {
                        string auctionGoodsJson = await RedisHelper.String.StringGetAsync("AuctionGoods_" + resultIndex[i].RedisKey);
                        if (auctionGoodsJson != null)
                        {
                            auctionGoodsDTOList.Add(await RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", resultIndex[i].RedisKey));
                            newResultIndex.Add(resultIndex[i]);
                        }
                    }
                    await RedisHelper.Hash.HashSetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsID.ToString(), newResultIndex);
            }
            return auctionGoodsDTOList;
        }
        /// <summary>
        /// 获取目标范围的商品信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<AuctionGoodsDTO>> GetTargetAuctionGoodList(List<AuctionGoodsDTO> allAuctionGoodsDTOs,int startIndex,int count)
        {
            int goodsCount = allAuctionGoodsDTOs.Count;
            List<AuctionGoodsDTO> resultAuctionGoodsDTOList = new List<AuctionGoodsDTO>();
            if (startIndex + count <= goodsCount)
            {
                resultAuctionGoodsDTOList = allAuctionGoodsDTOs.GetRange(startIndex, count);
            }
            else
            {
                resultAuctionGoodsDTOList = allAuctionGoodsDTOs.GetRange(startIndex, goodsCount - startIndex);
            }
            return resultAuctionGoodsDTOList;
        }
    }
}
