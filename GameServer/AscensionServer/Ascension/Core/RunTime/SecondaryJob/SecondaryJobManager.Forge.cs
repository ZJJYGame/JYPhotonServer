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
        #region  Redis模块   
        /// <summary>
        /// 学习新锻造配方
        /// </summary>
       async void UpdateForgeS2C(int roleID,int useItemID)
        {
            var formulaExist = GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaDrugData>>(out var formulaDataDict);
            if (!formulaExist)
            {
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateForge);
                return;
            }
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (ringServer == null)
            {
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateForge);
                return;
            }

            var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (InventoryManager.VerifyIsExist(useItemID, nHCriteriaRingID))
            {
                var forgeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                if (forgeExist)
                {
                    var forge = RedisHelper.Hash.HashGetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                    if (forge != null)
                    {
                        if (formulaDataDict.TryGetValue(useItemID, out var formula))
                        {
                            if (formula.FormulaLevel > forge.JobLevel)
                            {
                                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
                                return;
                            }

                            if (!forge.Recipe_Array.Contains(useItemID))
                            {
                                forge.Recipe_Array.Add(useItemID);

                                RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.UpdateAlchemy, forge);
                                InventoryManager.Remove(roleID, useItemID);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(forge));
                                await RedisHelper.Hash.HashSetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString(), forge);
                            }
                            else
                                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateAlchemy);
                        }
                    }
                    else
                        UpdateForgeMySql(roleID, useItemID, nHCriteria);
                }
                else
                    UpdateForgeMySql(roleID, useItemID, nHCriteria);
            }
            else
                UpdateForgeMySql(roleID, useItemID, nHCriteria);

        }
        /// <summary>
        /// 锻造武器法宝
        /// </summary>
        async void CompoundForge(int roleID, int useItemID)
        {
            var forgeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            var assestExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            var roleweaponExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            if (forgeExist&& roleExist&& assestExist&&roleweaponExist)
            {
                var forge = RedisHelper.Hash.HashGetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
                var assest = RedisHelper.Hash.HashGetAsync<RoleAssetsDTO>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                var roleweapon = RedisHelper.Hash.HashGetAsync<RoleWeaponDTO>(RedisKeyDefine._RoleWeaponPostfix, roleID.ToString()).Result;
                if (forge != null && role != null && assest != null&& roleweapon!=null)
                {
                    var forgeid = 0;//锻造出来的装备法宝唯一ID
                    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaDrugData>>(out var formulaDataDict);
                    if (forge.Recipe_Array.Contains(useItemID))
                    {
                        formulaDataDict.TryGetValue(useItemID, out var formulaData);
                        for (int i = 0; i < formulaData.NeedItemArray.Count; i++)
                        {
                            if (!InventoryManager.VerifyIsExist(formulaData.NeedItemArray[i], formulaData.NeedItemNumber[i], roleID))
                            {
                                RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundForge);
                                return;
                            }
                        }
                        if (formulaData.NeedMoney > assest.SpiritStonesLow || formulaData.NeedVitality > role.Vitality)
                        {
                            RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundForge);
                            return;
                        }
                        if (Utility.Algorithm.CreateRandomInt(0, 101) < formulaData.SuccessRate)
                        {
                            RoleStatusCompoundFailS2C(roleID, SecondaryJobOpCode.CompoundForge, default);
                            //鍛造失敗
                            return;
                        }
                        forge.JobLevelExp += formulaData.MasteryValue;
                        role.Vitality -= formulaData.NeedVitality;
                        assest.SpiritStonesLow -= formulaData.NeedMoney;

                        var weapobObj = ForgeStatusAlgorithm(useItemID,out string forgeType);
                        if (weapobObj!=null)
                        {
                            if (true)
                            {
                                var indexExist = roleweapon.Magicindex.TryGetValue(useItemID, out int id);
                                if (indexExist)
                                {
                                    roleweapon.Weaponindex.Add(useItemID, id + 1);
                                    roleweapon.WeaponStatusDict.Add(Convert.ToInt32(useItemID + "" + (id + 1)), weapobObj);
                                }
                            }
                            else
                            {
                                var indexExist = roleweapon.Magicindex.TryGetValue(useItemID, out int id);
                                if (indexExist)
                                {
                                    roleweapon.Magicindex.Add(useItemID, id + 1);
                                    roleweapon.MagicStatusDict.Add(Convert.ToInt32(useItemID + "" + (id + 1)), weapobObj);
                                }
                            }
                        }

                       await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._ForgePerfix, roleID.ToString(), roleweapon);
                        await NHibernateQuerier.UpdateAsync(ChangeDataType(roleweapon));

                       // InventoryManager.AddNewItem(roleID, forgeid, 1);
                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.JobForge, forge);
                        dict.Add((byte)ParameterCode.RoleAssets, assest);
                        dict.Add((byte)ParameterCode.RoleStatus, role);
                        RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.CompoundForge, dict);
                    }
                }
            }
        }
        #endregion

        #region MySql
        /// <summary>
        /// 学习新配方
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="useItemID"></param>
        /// <param name="nHCriteria"></param>
        async void UpdateForgeMySql(int roleID, int useItemID, NHCriteria nHCriteria)
        {
            var forge = NHibernateQuerier.CriteriaSelect<Forge>(nHCriteria);
            if (forge != null)
            {
                var recipe = Utility.Json.ToObject<List<int>>(forge.Recipe_Array);
                if (recipe.Contains(useItemID))
                {
                    recipe.Add(useItemID);
                    forge.Recipe_Array = Utility.Json.ToJson(recipe);
                    RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.UpdateForge, ChangeDataType(forge));
                    InventoryManager.Remove(roleID, useItemID);
                    await NHibernateQuerier.UpdateAsync(forge);
                    await RedisHelper.Hash.HashSetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString(), ChangeDataType(forge));
                }
                else
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateForge);
            }
            else
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.UpdateForge);
        }
        #endregion

        WeaponDTO ForgeStatusAlgorithm(int id,out string forgeType)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, ForgeParameter>>(out var forgeparameter);
            var result = forgeparameter.TryGetValue(id,out var parameter);
            forgeType = "";
            if (result)
            {
                WeaponDTO weapon = new WeaponDTO();
                for (int i = 0; i < parameter.WeaponAttributeMax.Count; i++)
                {
                    weapon.WeaponAttribute.Add(Utility.Algorithm.CreateRandomInt(parameter.WeaponAttributeMin[i], parameter.WeaponAttributeMax[i] + 1));
                }
                for (int i = 0; i < parameter.SkillProbability.Count; i++)
                {
                    var num = Utility.Algorithm.CreateRandomInt(0,101);
                    if (num<= parameter.SkillProbability[i])
                    {
                        weapon.WeaponSkill.Add(parameter.WeaponSkill[i]);
                    }
                }

                weapon.WeaponDurable = parameter.WeaponDurable;
                return weapon;
            }
            else
                return null;
        }


        ForgeDTO ChangeDataType(Forge forge)
        {
            ForgeDTO forgeDTO = new ForgeDTO();
            forgeDTO.RoleID = forge.RoleID;
            forgeDTO.JobLevel = forge.JobLevel;
            forgeDTO.JobLevelExp = forge.JobLevelExp;
            forgeDTO.Recipe_Array =Utility.Json.ToObject<HashSet<int>>(forge.Recipe_Array);
            return forgeDTO;
        }

        Forge ChangeDataType(ForgeDTO forgeDTO)
        {
            Forge forge = new Forge();
            forge.RoleID = forgeDTO.RoleID;
            forge.JobLevel = forgeDTO.JobLevel;
            forge.JobLevelExp = forgeDTO.JobLevelExp;
            forge.Recipe_Array = Utility.Json.ToJson(forge.Recipe_Array);
            return forge;
        }

        RoleWeapon ChangeDataType(RoleWeaponDTO weaponDTO)
        {
            RoleWeapon weapon = new RoleWeapon();
            weapon.RoleID = weaponDTO.RoleID;
            weapon.Magicindex = Utility.Json.ToJson(weaponDTO.Magicindex);
            weapon.MagicStatusDict = Utility.Json.ToJson(weaponDTO.MagicStatusDict);
            weapon.Weaponindex = Utility.Json.ToJson(weaponDTO.Weaponindex);
            weapon.MagicStatusDict = Utility.Json.ToJson(weaponDTO.MagicStatusDict);
            return weapon;
        }
    }
}
