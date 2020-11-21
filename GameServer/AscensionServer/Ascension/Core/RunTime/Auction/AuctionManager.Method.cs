using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using RedisDotNet;
using Cosmos;
using AscensionServer.Model;

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

        /// <summary>
        /// 玩家购买物品的事件处理
        /// </summary>
        /// <param name="buyAuctionGoodsDTO"></param>
        public async void BuyAuctionGoods(AuctionGoodsDTO buyAuctionGoodsDTO,int buyerId)
        {
            if (occupyGuidHash.Contains(buyAuctionGoodsDTO.GUID))
            {
                return;
            }
            else
            {
                occupyGuidHash.Add(buyAuctionGoodsDTO.GUID);
                if(!await IsAuctionGoodsExist(buyAuctionGoodsDTO.GUID))//商品不存在，返回
                {
                    return;
                }
                else//商品存在，尝试去处理拍卖品，扣除金钱，添加背包
                {
                    //拍卖行对应拍卖品
                    AuctionGoodsDTO auctionGoodsObj = null;
                    AuctionGoodsDTO resultAuctionGoodsDTO = null;
                    auctionGoodsObj = await RedisHelper.Hash.HashGetAsync<AuctionGoodsDTO>("AuctionGoodsData", buyAuctionGoodsDTO.GUID);
                    if (buyAuctionGoodsDTO.Count <= auctionGoodsObj.Count)
                    {
                        resultAuctionGoodsDTO = new AuctionGoodsDTO()
                        {
                            GUID = auctionGoodsObj.GUID,
                            RoleID = auctionGoodsObj.RoleID,
                            GlobalID = auctionGoodsObj.GlobalID,
                            Price = auctionGoodsObj.Price,
                            ItemData = auctionGoodsObj.ItemData,
                            Count = buyAuctionGoodsDTO.Count
                        };
                        auctionGoodsObj.Count -= buyAuctionGoodsDTO.Count;
                        if (auctionGoodsObj.Count == 0)//如果商品被买完了
                        {
                            await RedisHelper.KeyDeleteAsync("AuctionGoods_" + auctionGoodsObj.GUID);
                            await RedisHelper.Hash.HashDeleteAsync("AuctionGoodsData", buyAuctionGoodsDTO.GUID);
                            List<string> roleAuctionGuidList =await RedisHelper.Hash.HashGetAsync<List<string>>("RoleAuctionItems", auctionGoodsObj.RoleID.ToString());
                            string guidStr = roleAuctionGuidList.Find((p) => p == auctionGoodsObj.GUID);
                            roleAuctionGuidList.Remove(guidStr);
                            if (roleAuctionGuidList.Count != 0)
                            {
                                await RedisHelper.Hash.HashSetAsync("RoleAuctionItems", auctionGoodsObj.RoleID.ToString(), roleAuctionGuidList);
                            }
                            else
                            {
                                await RedisHelper.Hash.HashDeleteAsync("RoleAuctionItems", auctionGoodsObj.RoleID.ToString());
                            }
                            //处理同种商品的拍卖品列表
                            List<AuctionGoodsIndex> tempAuctionGoodIndexs = await RedisHelper.Hash.HashGetAsync<List<AuctionGoodsIndex>>("AuctionIndex", auctionGoodsObj.GlobalID.ToString());
                            AuctionGoodsIndex auctionGoodsIndex = tempAuctionGoodIndexs.Find((p) => p.RedisKey == auctionGoodsObj.GUID);
                            tempAuctionGoodIndexs.Remove(auctionGoodsIndex);
                            if (tempAuctionGoodIndexs.Count != 0)//购买后当前种物品索引数量 不为0
                            {
                                await RedisHelper.Hash.HashSetAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString(), tempAuctionGoodIndexs);
                            }
                            else
                            {
                                await RedisHelper.Hash.HashDeleteAsync("AuctionIndex", auctionGoodsObj.GlobalID.ToString());
                            }
                        }
                        else
                        {
                            await RedisHelper.Hash.HashSetAsync("AuctionGoodsData", buyAuctionGoodsDTO.GUID, auctionGoodsObj);
                        }
                    }
                    else//购买数量多于商品数量，返回
                    {
                        return;
                    }
                    //todo
                    //扣除买家金钱
                    ChangeRoleAssets(auctionGoodsObj, buyerId, false);
                    //增加卖家金钱
                    ChangeRoleAssets(auctionGoodsObj, buyerId, true);
                    //告知卖家
                    //物品赋予买家
                }
            }
        }
        /// <summary>
        /// 角色金钱修改
        /// </summary>
        public async void ChangeRoleAssets(AuctionGoodsDTO  buyAuctionGoodsDTO,int targetRoleId,bool isAdd)
        {
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", targetRoleId);
            bool roleExist = NHibernateQuerier.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = NHibernateQuerier.Verify<RoleAssets>(nHCriteriaRoleID);
            if (roleExist && roleAssetsExist)
            {
                var assetsServer = NHibernateQuerier.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);
                assetsServer.SpiritStonesLow += buyAuctionGoodsDTO.Price * buyAuctionGoodsDTO.Count * (isAdd?100:-100);
                NHibernateQuerier.Update<RoleAssets>(new RoleAssets() { RoleID = targetRoleId, SpiritStonesLow = assetsServer.SpiritStonesLow, XianYu = assetsServer.XianYu });
                await RedisHelper.Hash.HashSetAsync<RoleAssets>("RoleAssets", targetRoleId.ToString(), new RoleAssets() { RoleID = targetRoleId, SpiritStonesLow = assetsServer.SpiritStonesLow, XianYu = assetsServer.XianYu });
            }
        }

    }
}
