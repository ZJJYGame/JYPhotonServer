using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public partial class GangsMananger
    {
        #region redis模块
        /// <summary>
        /// 获得宗门物品设置
        /// </summary>
        void GetExchangeGoodsS2C(int roleID, int id)
        {
            var ExchangeExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix, id.ToString()).Result;
            if (ExchangeExit)
            {
                var ExchangeObj = RedisHelper.Hash.HashGetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, id.ToString()).Result;
                if (ExchangeObj != null)
                {
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetExchangeGoods, ExchangeObj);
                }
                else
                    GetExchangeGoodsMySql(roleID, id);
            }
            else
                GetExchangeGoodsMySql(roleID, id);
        }
        /// <summary>
        /// 设置宗门物品兑换
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="goodsDTO"></param>
        async void SetExchangeGoodsS2C(int roleID, AllianceExchangeGoodsDTO goodsDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, AllianceScripturesPlatformData>>(out var exchangeDict);

            var ExchangeExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString()).Result;
            var roleExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
            if (ExchangeExit&& roleExit)
            {
                var ExchangeObj= RedisHelper.Hash.HashGetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString()).Result;
                var roleObj = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix,roleID.ToString()).Result;
                if (ExchangeObj != null && roleObj != null)
                {
                    foreach (var item in goodsDTO.ExchangeGoods)
                    {
                        if (exchangeDict.TryGetValue(item.Key, out var data))
                        {
                            var result = ExchangeObj.ExchangeGoods.ContainsKey(item.Key);
                            if (!result && item.Value.Contribution <= data.ContributionUp && item.Value.Contribution <= data.ContributionDown)
                            {
                                ExchangeObj.ExchangeGoods.Add(item.Key, item.Value);
                            }
                        }
                    }

                    RoleStatusSuccessS2C(roleID, AllianceOpCode.SetExchangeGoods, ExchangeObj);
                    await RedisHelper.Hash.HashSetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString(), ExchangeObj);
                    await NHibernateQuerier.UpdateAsync(ChangeDataType(ExchangeObj));
                }
                else
                    SetExchangeGoodsMySql(roleID, goodsDTO);
            }
            else
                SetExchangeGoodsMySql(roleID, goodsDTO);
        }
        #endregion

        #region Mysql模块
        /// <summary>
        /// 获得宗门物品设置
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="goodsDTO"></param>
        void GetExchangeGoodsMySql(int roleID, int id)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", id);
            var ExchangeObj = NHibernateQuerier.CriteriaSelect<AllianceExchangeGoods>(nHCriteriaAlliance);
            Utility.Debug.LogInfo("获得角色宗門设置数据1");
            if (ExchangeObj != null)
            {
                Utility.Debug.LogInfo("获得角色宗門设置数据2");
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetExchangeGoods, ChangeDataType (ExchangeObj));
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetExchangeGoods);
        }

        async void SetExchangeGoodsMySql(int roleID, AllianceExchangeGoodsDTO goodsDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, AllianceScripturesPlatformData>>(out var exchangeDict);
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", goodsDTO.AllianceID);
            var ExchangeObj = NHibernateQuerier.CriteriaSelectAsync<AllianceExchangeGoods>(nHCriteriaAlliance).Result;
            if (ExchangeObj != null)
            {
                Utility.Debug.LogInfo("角色宗門设置兑换数据2");
                var goodsDict = Utility.Json.ToObject<Dictionary<int, ExchangeSetting>>(ExchangeObj.ExchangeGoods);
                foreach (var item in goodsDTO.ExchangeGoods)
                {
                    if (exchangeDict.TryGetValue(item.Key, out var data))
                    {
                        var result = goodsDict.ContainsKey(item.Key);
                        if (!result && item.Value.Contribution <= data.ContributionUp && item.Value.Contribution <= data.ContributionDown)
                        {
                            goodsDict.Add(item.Key, item.Value);
                        }
                    }
                }
                ExchangeObj.ExchangeGoods = Utility.Json.ToJson(goodsDict);

                RoleStatusSuccessS2C(roleID,AllianceOpCode.SetExchangeGoods, ChangeDataType(ExchangeObj));
                await RedisHelper.Hash.HashSetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString(), ChangeDataType(ExchangeObj));
                await NHibernateQuerier.UpdateAsync(ExchangeObj);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.SetExchangeGoods);

        }
        #endregion 

        AllianceExchangeGoods ChangeDataType(AllianceExchangeGoodsDTO goodsDTO)
        {
            AllianceExchangeGoods alliance = new AllianceExchangeGoods();
            alliance.AllianceID = goodsDTO.AllianceID;
            alliance.ExchangeGoods = Utility.Json.ToJson(goodsDTO.ExchangeGoods);
            return alliance;
        }

        AllianceExchangeGoodsDTO ChangeDataType(AllianceExchangeGoods goodsDTO)
        {
            AllianceExchangeGoodsDTO alliance = new AllianceExchangeGoodsDTO();
            alliance.AllianceID = goodsDTO.AllianceID;
            alliance.ExchangeGoods = Utility.Json.ToObject<Dictionary<int, ExchangeSetting>>(goodsDTO.ExchangeGoods);
            return alliance;
        }
    }
}
