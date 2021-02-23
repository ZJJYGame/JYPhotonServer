using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public interface IAuctionManager:IModuleManager
    {
        Task<bool> IsAuctionGoodsExist(string Guid);
        void ServerAuctionGoodsGet(int auctionGoodsID, int startIndex, int count, int roleId);
        /// <summary>
        /// 购买拍卖品
        /// </summary>
        void SeverAuctionGoodsBuy(AuctionGoodsDTO auctionGoodsDTO, int startIndex, int count, int roleId);
        /// <summary>
        /// 上架拍卖品
        /// </summary>
        void SeverAuctionGoodsPutAway(AuctionGoodsDTO putAwayAuctionGoods, int itemId, int roleId);
        /// <summary>
        /// 获取某一种物品的所有物品信息
        /// </summary>
        /// <param name="auctionGoodsID"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<List<AuctionGoodsDTO>> GetAuctionGoodsList(int auctionGoodsID);
        /// <summary>
        /// 获取目标范围的商品信息
        /// </summary>
        /// <returns></returns>
        List<AuctionGoodsDTO> GetTargetAuctionGoodList(List<AuctionGoodsDTO> allAuctionGoodsDTOs, int startIndex, int count);
        /// <summary>
        /// 玩家购买物品的事件处理
        /// </summary>
        /// <param name="buyAuctionGoodsDTO"></param>
        /// 0=》别人购买中
        /// 1=>商品不存在
        /// 2=》数量不足
        /// 3=》购买成功
        /// 4=》意外情况
        Task<byte> BuyAuctionGoods(AuctionGoodsDTO buyAuctionGoodsDTO, int buyerId);
        /// <summary>
        /// 改变玩家金钱
        /// </summary>
        /// <param name="spiritStonePrice"></param>
        /// <param name="xianYuPrice"></param>
        /// <param name="targetRoleId"></param>
        /// <returns></returns>
        Task ChangeRoleAssets(int spiritStonePrice, int xianYuPrice, int targetRoleId);
        /// <summary>
        /// 玩家上架拍卖品的事件
        /// </summary>
        Task PutAwayAuctionGoods(AuctionGoodsDTO putAwayGoods, int itemId);
    }
}


