using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    public partial class SecondaryJobManager
    {
        #region Redis 模塊
        /// <summary>
        /// 获得所有已学配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
         void GetAlchemyStatusS2C(int roleID)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
            if (result)
            {
                var alchemytemp = RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;

                if (alchemytemp != null)
                {
                    RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.GetAlchemyStatus, alchemytemp);
                }
                else
                    GetAlchemyStatusMySql(roleID);
            }
            else
                GetAlchemyStatusMySql(roleID);
        }
        /// <summary>
        /// 学习新配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
         async void UpdateAlchemyS2C(int roleID, int UseItemID)
        {
            #region
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (ringServer==null)
            {
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
                return;
            }
            var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);

            if (InventoryManager.VerifyIsExist(UseItemID, nHCriteriaRingID))
            {
                var alchemyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix,roleID.ToString()).Result;
                if (alchemyExist)
                {
                    var alchemy = RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
                    if (alchemy != null)
                    {
                        if (alchemy.Recipe_Array.Contains(UseItemID))
                        {
                            alchemy.Recipe_Array.Add(UseItemID);

                            RoleStatusSuccessS2C(roleID,SecondaryJobOpCode.UpdateAlchemy, alchemy);
                            InventoryManager.Remove(roleID,UseItemID);
                            await NHibernateQuerier.UpdateAsync(ChangeDataType(alchemy));
                            await RedisHelper.Hash.HashSetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPostfix, roleID.ToString(), alchemy);
                        }
                        else
                            RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
                    }
                    else
                        UpdateAlchemyMySql(roleID, UseItemID, nHCriteria);
                }
                else
                    UpdateAlchemyMySql(roleID, UseItemID, nHCriteria);
            }
            else
                RoleStatusFailS2C(roleID,SecondaryJobOpCode.UpdateAlchemy);
            #endregion
        }
        /// <summary>
        /// 合成配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        void CompoundAlchemyS2C(int roleID, int UseItemID)
        {
            #region
            var alchemyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            var assestExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            if (alchemyExist&& assestExist&& roleExist)
            {
                var alchemy = RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
                var assest = RedisHelper.Hash.HashGetAsync<RoleAssetsDTO>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                if (alchemy != null&& role!=null&& assest!=null)
                {
                    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaData>>(out var formulaDataDict);
                    if (alchemy.Recipe_Array.Contains(UseItemID))
                    {
                        formulaDataDict.TryGetValue(UseItemID, out var formulaData);
                        for (int i = 0; i < formulaData.NeedItemArray.Count; i++)
                        {
                            if (!InventoryManager.VerifyIsExist(formulaData.NeedItemArray[i], formulaData.NeedItemNumber[i],roleID))
                            {
                                RoleStatusFailS2C(roleID,SecondaryJobOpCode.CompoundAlchemy);
                                return;
                            }                         
                        }
                        if (formulaData.NeedMoney> assest.SpiritStonesLow||formulaData.NeedVitality > role.Vitality)
                        {
                            RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundAlchemy);
                            return;
                        }
                        if (Utility.Algorithm.CreateRandomInt(0, 101) < formulaData.SuccessRate)
                        {
                            RoleStatusCompoundFailS2C(roleID, SecondaryJobOpCode.CompoundAlchemy,default);
                            //鍛造失敗
                            return;
                        }

                        alchemy.JobLevelExp += formulaData.MasteryValue;
                        role.Vitality -= formulaData.NeedVitality;
                        assest.SpiritStonesLow -= formulaData.NeedMoney;
                        InventoryManager.AddNewItem(roleID,formulaData.ItemID,1);
                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.JobAlchemy, alchemy);
                        dict.Add((byte)ParameterCode.RoleAssets, assest);
                        dict.Add((byte)ParameterCode.RoleStatus, role);
                        RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.CompoundAlchemy, dict);
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID,SecondaryJobOpCode.CompoundAlchemy);
                    }
                }
                else
                {
                   //mysql邏輯
                }

                #endregion
            }
        }
  
        #endregion

        #region MysSql模塊
        void GetAlchemyStatusMySql(int roleID)
        {
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteria);
            if (alchemy != null)
            {
                RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.GetAlchemyStatus, ChangeDataType(alchemy));
            }
        }
        /// <summary>
        /// 學習新配方
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="UseItemID"></param>
        /// <param name="nHCriteria"></param>
        async void UpdateAlchemyMySql(int roleID, int UseItemID, NHCriteria nHCriteria)
        {
            var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteria);
            if (alchemy != null)
            {
                var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
                if (recipe.Contains(UseItemID))
                {
                    recipe.Add(UseItemID);
                    alchemy.Recipe_Array = Utility.Json.ToJson(recipe);
                    RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.UpdateAlchemy, ChangeDataType(alchemy));
                    InventoryManager.Remove(roleID, UseItemID);
                    await NHibernateQuerier.UpdateAsync(alchemy);
                    await RedisHelper.Hash.HashSetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPostfix, roleID.ToString(), ChangeDataType(alchemy));
                }
                else
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
            }
            else
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
        }
        /// <summary>
        /// 煉丹
        /// </summary>
        async void CompoundAlchemyM有SQL()
        {
            #region
            //NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            //var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteria);
            //if (alchemy != null)
            //{
            //    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaData>>(out var formulaDataDict);
            //    var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
            //    if (recipe.Contains(secondaryJobDTO.UseItemID))
            //    {
            //        formulaDataDict.TryGetValue(secondaryJobDTO.UseItemID, out var formulaData);

            //    }
            //    else
            //    {
            //        S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail);
            //    }
            //}
            //else
            //{
            //    S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail);
            //}

            #endregion
        }
        #endregion

        AlchemyDTO ChangeDataType(Alchemy alchemy)
        {
            AlchemyDTO alchemyDTO = new AlchemyDTO();
            alchemyDTO.RoleID = alchemy.RoleID;
            alchemyDTO.JobLevel = alchemy.JobLevel;
            alchemyDTO.JobLevelExp = alchemy.JobLevelExp;
            alchemyDTO.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemy.Recipe_Array);
            return alchemyDTO;
        }

        Alchemy ChangeDataType(AlchemyDTO alchemyDTO)
        {
            Alchemy alchemy = new Alchemy();
            alchemy.RoleID = alchemy.RoleID;
            alchemy.JobLevel = alchemy.JobLevel;
            alchemy.JobLevelExp = alchemy.JobLevelExp;
            alchemy.Recipe_Array = Utility.Json.ToJson(alchemy.Recipe_Array);
            return alchemy;
        }
    }
}


