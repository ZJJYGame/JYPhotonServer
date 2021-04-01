﻿using AscensionProtocol;
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
            var ExchangeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix, id.ToString()).Result;
            if (ExchangeExist)
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

            var ExchangeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
            if (ExchangeExist&& roleExist)
            {
                var ExchangeObj= RedisHelper.Hash.HashGetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString()).Result;
                var roleObj = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix,roleID.ToString()).Result;
                if (ExchangeObj != null && roleObj != null)
                {
                    if (roleObj.AllianceJob==937)
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
                                else
                                {
                                    ExchangeObj.ExchangeGoods[item.Key] = item.Value;
                                }
                            }
                        }

                        RoleStatusSuccessS2C(roleID, AllianceOpCode.SetExchangeGoods, ExchangeObj);
                        await RedisHelper.Hash.HashSetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString(), ExchangeObj);
                        await NHibernateQuerier.UpdateAsync(ChangeDataType(ExchangeObj));
                    }else
                        RoleStatusFailS2C(roleID, AllianceOpCode.SetExchangeGoods);
                }
                else
                    SetExchangeGoodsMySql(roleID, goodsDTO);
            }
            else
                SetExchangeGoodsMySql(roleID, goodsDTO);
        }
        /// <summary>
        /// 兑换丹药
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="id"></param>
        /// <param name="drugDTO"></param>
        async void ExchangeElixirS2C(int roleid,int id, ExchangeDTO drugDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, AllianceDrugData>>(out var drugDict);
            var roleallianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix,roleid.ToString()).Result;
            var roleassetsExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleid.ToString()).Result;
            var constructionExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleid.ToString()).Result;
            if (!drugDict.TryGetValue(drugDTO.GoodsID,out var data) )
            {
                RoleStatusFailS2C(roleid,AllianceOpCode.ExchangeElixir);
                return;
            }
            if (roleallianceExist && roleassetsExist && constructionExist)
            {
                var construction = RedisHelper.Hash.HashGetAsync<AllianceConstruction>(RedisKeyDefine._AllianceConstructionPerfix, id.ToString()).Result;
                var rolealliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleid.ToString()).Result;
                var roleassets = RedisHelper.Hash.HashGetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleid.ToString()).Result;
                if (rolealliance != null && roleassets != null && construction != null)
                {
                    if (rolealliance.Reputation >= (data.AllianceContribution * drugDTO.GoodsNum) && roleassets.SpiritStonesLow >= data.SpiritStones && construction.AllianceAlchemyStorage >= data.DrugHouseLevel)
                    {
                        InventoryManager.AddNewItem(roleid, drugDTO.GoodsID, drugDTO.GoodsNum);
                        rolealliance.Reputation -= (data.AllianceContribution * drugDTO.GoodsNum);
                        roleassets.SpiritStonesLow -= data.SpiritStones;
                        construction.AllianceAlchemyStorage -= data.DrugHouseLevel;

                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.RoleAlliance, rolealliance);
                        dict.Add((byte)ParameterCode.RoleAssets, roleassets);
                        RoleStatusSuccessS2C(roleid, AllianceOpCode.ExchangeElixir, dict);

                        await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, id.ToString(), rolealliance);
                        await RedisHelper.Hash.HashSetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, id.ToString(), roleassets);
                        await  NHibernateQuerier.UpdateAsync(roleassets);
                        await NHibernateQuerier.UpdateAsync(ChangeDataType(rolealliance));
                    }
                }
            }
            else
            {
                ExchangeElixirMySql(roleid,id, drugDTO);
            }


        }
        /// <summary>
        /// 兑换功法
        /// </summary>
        async void ExchangeScripturesPlatformS2C(int roleid, int id, ExchangeDTO goods)
        {
            var exchangeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix,id.ToString()).Result;
            var roleallianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix, roleid.ToString()).Result;
            if (exchangeExist&& roleallianceExist)
            {
                Utility.Debug.LogInfo("YZQ兌換藏經閣數據2");
                var exchange = RedisHelper.Hash.HashGetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, id.ToString()).Result;
                var rolealliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleid.ToString()).Result;
                if (exchange != null && rolealliance != null)
                {
                    if (exchange.ExchangeGoods.TryGetValue(goods.GoodsID, out var data))
                    {
                        if (rolealliance.AllianceJob >= data.Job && rolealliance.Reputation >= data.Contribution)
                        {
                            rolealliance.Reputation -= data.Contribution;
                            InventoryManager.AddNewItem(roleid, goods.GoodsID, goods.GoodsNum);

                            RoleStatusSuccessS2C(roleid, AllianceOpCode.ExchangeScripturesPlatform, rolealliance);

                            await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleid.ToString(), rolealliance);
                            await NHibernateQuerier.UpdateAsync(ChangeDataType(rolealliance));
                        }
                    }
                    else
                    {
                        RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeScripturesPlatform);
                    }
                }
                else
                    ExchangeScripturesPlatformMySql(roleid,id, goods);
            }
            else
                ExchangeScripturesPlatformMySql(roleid, id, goods);
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
            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var roleObj = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriarole).Result;
            if (ExchangeObj != null&& roleObj!=null)
            {
                if (roleObj.AllianceJob==937)
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

                    RoleStatusSuccessS2C(roleID, AllianceOpCode.SetExchangeGoods, ChangeDataType(ExchangeObj));
                    await RedisHelper.Hash.HashSetAsync<AllianceExchangeGoodsDTO>(RedisKeyDefine._AllianceExchangeGoodsPerfix, goodsDTO.AllianceID.ToString(), ChangeDataType(ExchangeObj));
                    await NHibernateQuerier.UpdateAsync(ExchangeObj);
                }
                else
                    RoleStatusFailS2C(roleID, AllianceOpCode.SetExchangeGoods);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.SetExchangeGoods);

        }
        /// <summary>
        /// 兑换丹药
        /// </summary>
        async void ExchangeElixirMySql(int roleid,int id, ExchangeDTO goodsDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, AllianceDrugData>>(out var drugDict);
            if (!drugDict.TryGetValue(goodsDTO.GoodsID, out var data))
            {
                RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeElixir);
                return;
            }
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", id);
            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var rolealliance = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriarole).Result;
            var roleassets = NHibernateQuerier.CriteriaSelectAsync<RoleAssets>(nHCriteriarole).Result;
            var construction = NHibernateQuerier.CriteriaSelectAsync<AllianceConstruction>(nHCriteriaAlliance).Result;
            if (construction != null&& roleassets != null&& rolealliance != null)
            {
                Utility.Debug.LogInfo("YZQ兑换宗门丹药2");
                if (rolealliance.Reputation >= (data.AllianceContribution * goodsDTO.GoodsNum) && roleassets.SpiritStonesLow >= data.SpiritStones && construction.AllianceAlchemyStorage >= data.DrugHouseLevel)
                {
                    InventoryManager.AddNewItem(roleid, goodsDTO.GoodsID, goodsDTO.GoodsNum);
                    rolealliance.Reputation -= (data.AllianceContribution * goodsDTO.GoodsNum);
                    roleassets.SpiritStonesLow -= data.SpiritStones;
                    construction.AllianceAlchemyStorage -= data.DrugHouseLevel;

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.RoleAlliance, ChangeDataType(rolealliance));
                    dict.Add((byte)ParameterCode.RoleAssets, roleassets);
                    RoleStatusSuccessS2C(roleid, AllianceOpCode.ExchangeElixir, dict);

                    await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, id.ToString(), ChangeDataType(rolealliance));
                    await RedisHelper.Hash.HashSetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, id.ToString(), roleassets);
                    await NHibernateQuerier.UpdateAsync(roleassets);
                    await NHibernateQuerier.UpdateAsync(rolealliance);
                }else
                    RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeElixir);
            }else
                RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeElixir);

        }

        async void ExchangeScripturesPlatformMySql(int roleid, int id, ExchangeDTO goods)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", id);
            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var exchange = NHibernateQuerier.CriteriaSelectAsync<AllianceExchangeGoods>(nHCriteriaAlliance).Result;
            var rolealliance = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriarole).Result;
            if (exchange != null && rolealliance != null)
            {
                Utility.Debug.LogInfo("YZQ兌換藏經閣數據3");
                var goodDict = Utility.Json.ToObject<Dictionary<int, ExchangeSetting>>(exchange.ExchangeGoods);

                if (goodDict.TryGetValue(goods.GoodsID, out var data))
                {
                    if (rolealliance.AllianceJob >= data.Job && rolealliance.Reputation >= data.Contribution)
                    {
                        rolealliance.Reputation -= data.Contribution;
                        InventoryManager.AddNewItem(roleid, goods.GoodsID, goods.GoodsNum);

                        RoleStatusSuccessS2C(roleid, AllianceOpCode.ExchangeScripturesPlatform, ChangeDataType(rolealliance));

                        await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleid.ToString(), ChangeDataType(rolealliance));
                        await NHibernateQuerier.UpdateAsync(rolealliance);
                    }
                }
                else
                {
                    RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeScripturesPlatform);
                }
            }
            else
            {
                RoleStatusFailS2C(roleid, AllianceOpCode.ExchangeScripturesPlatform);
            }
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
