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
            var formulaExist = GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaForgeData>>(out var formulaDataDict);
            if (!formulaExist)
            {
                Utility.Debug.LogInfo("YZQ添加锻造配方请求1");
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);
                return;
            }
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (ringServer == null)
            {
                Utility.Debug.LogInfo("YZQ添加锻造配方请求2");
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);
                return;
            }

            if (InventoryManager.VerifyIsExist(useItemID, 1, ringServer.RingIdArray))
            {
                var tempid = Utility.Converter.RetainInt32(useItemID, 5);
                var forgeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                if (forgeExist)
                {
                    var forge = RedisHelper.Hash.HashGetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                    if (forge != null)
                    {
                        if (formulaDataDict.TryGetValue(tempid, out var formula))
                        {
                            if (formula.NeedJobLevel > forge.JobLevel)
                            {
                                Utility.Debug.LogInfo("YZQ添加锻造配方请求3");
                                RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);
                                return;
                            }

                            if (!forge.Recipe_Array.Contains(tempid))
                            {
                                forge.Recipe_Array.Add(tempid);
                                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                                dict.Add((byte)ParameterCode.JobForge, forge);

                                RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus, dict);
                                InventoryManager.Remove(roleID, useItemID);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(forge));
                                await RedisHelper.Hash.HashSetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString(), forge);
                            }
                            else { Utility.Debug.LogInfo("YZQ添加锻造配方请求4"); RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus); }                    
                        }
                    }
                    else
                        UpdateForgeMySql(roleID, useItemID, nHCriteria);
                }
                else
                    UpdateForgeMySql(roleID, useItemID, nHCriteria);
            }
            else
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);

        }
        /// <summary>
        /// 锻造武器法宝
        /// </summary>
        async void CompoundForge(int roleID, int useItemID)
        {
            var forgeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            var assestExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            var roleweaponExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleWeaponPostfix, roleID.ToString()).Result;
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (forgeExist&& roleExist&& assestExist&&roleweaponExist)
            {
                var forge = RedisHelper.Hash.HashGetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
                var assest = RedisHelper.Hash.HashGetAsync<RoleAssetsDTO>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                var roleweapon = RedisHelper.Hash.HashGetAsync<RoleWeaponDTO>(RedisKeyDefine._RoleWeaponPostfix, roleID.ToString()).Result;
                if (forge != null && role != null && assest != null&& roleweapon!=null)
                {
                    var forgeid = 0;//锻造出来的装备法宝唯一ID
                    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaForgeData>>(out var formulaDataDict);
                    if (forge.Recipe_Array.Contains(useItemID))
                    {
                        formulaDataDict.TryGetValue(useItemID, out var formulaData);
                        for (int i = 0; i < formulaData.NeedItemArray.Count; i++)
                        {
                            if (!InventoryManager.VerifyIsExist(formulaData.NeedItemArray[i], formulaData.NeedItemNumber[i], ringServer.RingIdArray))
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
                        //for (int i = 0; i < 100; i++)
                        //{
                        //    var randNum = GameEntry.PetStatusManager.NormalRandom2(0, 101);
                        //    Utility.Debug.LogInfo("YZQ鍛造失敗随机数：" + randNum);
                        //}
                        var randNum = NormalRandom2(40,61);
                        if (randNum <= 30&& randNum>=70)
                        {
                            RoleStatusCompoundFailS2C(roleID, SecondaryJobOpCode.CompoundForge, default);
                            Utility.Debug.LogInfo("YZQ鍛造失敗随机数：" + randNum + "成功率：" + formulaData.SuccessRate);
                            return;
                        }
                        forge.JobLevelExp += formulaData.MasteryValue;
                        role.Vitality -= formulaData.NeedVitality;
                        assest.SpiritStonesLow -= formulaData.NeedMoney;

                        var weapobObj = ForgeStatusAlgorithm(formulaData.ItemID);
                        if (weapobObj != null)
                        {
                            if (formulaData.SyntheticType != 9)//锻造的非法宝装备
                            {
                                var indexExist = roleweapon.Weaponindex.TryGetValue(formulaData.ItemID, out int id);
                                if (indexExist)
                                {
                                    roleweapon.Weaponindex[formulaData.ItemID] = id + 1;
                                    roleweapon.WeaponStatusDict.Add(Convert.ToInt32(formulaData.ItemID + "" + (id + 1)), weapobObj);
                                    forgeid = Convert.ToInt32(formulaData.ItemID + "" + (id + 1));
                                }
                                else
                                {
                                    roleweapon.Weaponindex.Add(formulaData.ItemID, 1);
                                    roleweapon.WeaponStatusDict.Add(Convert.ToInt32(formulaData.ItemID + "" + 1), weapobObj);
                                    forgeid = Convert.ToInt32(formulaData.ItemID + "" + 1);
                                }
                            }
                            else
                            {
                                var indexExist = roleweapon.Magicindex.TryGetValue(formulaData.ItemID, out int id);
                                if (indexExist)
                                {
                                    roleweapon.Magicindex[formulaData.ItemID] = id + 1;
                                    roleweapon.MagicStatusDict.Add(Convert.ToInt32(formulaData.ItemID + "" + (id + 1)), weapobObj);
                                    forgeid = Convert.ToInt32(formulaData.ItemID + "" + (id + 1));
                                }
                                else
                                {
                                    roleweapon.Magicindex.Add(formulaData.ItemID, 1);
                                    roleweapon.MagicStatusDict.Add(Convert.ToInt32(formulaData.ItemID + "" + 1), weapobObj);
                                    forgeid = Convert.ToInt32(formulaData.ItemID + "" + 1);
                                }
                            }

                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleWeaponPostfix, roleID.ToString(), roleweapon);
                            await NHibernateQuerier.UpdateAsync(ChangeDataType(roleweapon));

                            InventoryManager.AddNewItem(roleID, forgeid, 1);
                            Dictionary<byte, object> dict = new Dictionary<byte, object>();
                            dict.Add((byte)ParameterCode.JobForge, forge);
                            dict.Add((byte)ParameterCode.RoleAssets, assest);
                            dict.Add((byte)ParameterCode.RoleStatus, role);
                            RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.CompoundForge, dict);
                        }
                        else
                            RoleStatusCompoundFailS2C(roleID, SecondaryJobOpCode.CompoundForge, default);
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
            Utility.Debug.LogInfo("YZQ添加锻造配方请求5");
            var forge = NHibernateQuerier.CriteriaSelect<Forge>(nHCriteria);
            if (forge != null)
            {
                var tempid = Utility.Converter.RetainInt32(useItemID, 5);
                var recipe = Utility.Json.ToObject<List<int>>(forge.Recipe_Array);
                if (!recipe.Contains(tempid))
                {
                    Utility.Debug.LogInfo("YZQ添加锻造配方请求6");
                    recipe.Add(tempid);
                    forge.Recipe_Array = Utility.Json.ToJson(recipe);
                    RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus, ChangeDataType(forge));
                    InventoryManager.Remove(roleID, useItemID);
                    await NHibernateQuerier.UpdateAsync(forge);
                    await RedisHelper.Hash.HashSetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString(), ChangeDataType(forge));
                }
                else {
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);
                }
                   
            }
            else
                RoleStatusFailS2C(roleID, SecondaryJobOpCode.StudySecondaryJobStatus);
        }      
        #endregion
        /// <summary>
        /// 装备锻造属性计算
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WeaponDTO ForgeStatusAlgorithm(int id)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, ForgeParameter>>(out var forgeparameter);
            var result = forgeparameter.TryGetValue(id,out var parameter);
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
            weapon.WeaponStatusDict = Utility.Json.ToJson(weaponDTO.WeaponStatusDict);
            return weapon;
        }

        #region 真正的正态分布策划版
        int[,] Map = new int[25, 10]
        {
            {5000,5040,5080,5120,5160,5199,5239,5279,5319,5359},
            {5398,5438,5478,5517,5557,5596,5636,5675,5714,5753},
            {5793,5832,5871,5910,5948,5987,6026,6064,6103,6141},
            {6179,6217,6255,6293,6331,6368,6406,6443,6480,6517},
            {6554,6591,6628,6664,6700,6736,6772,6808,6844,6879},
            {6915,6950,6985,7019,7054,7088,7123,7157,7190,7224},
            {7257,7291,7324,7357,7389,7422,7454,7486,7517,7549},
            {7580,7611,7642,7673,7703,7734,7764,7794,7823,7852},
            {7881,7910,7939,7967,7995,8023,8051,8078,8106,8133},
            {8159,8186,8212,8238,8264,8289,8315,8340,8365,8389},
            {8413,8438,8461,8485,8508,8531,8554,8577,8599,8621},
            {8643,8665,8686,8708,8729,8749,8770,8790,8810,8830},
            {8849,8869,8888,8907,8925,8944,8962,8980,8997,9015},
            {9032,9049,9066,9082,9099,9115,9131,9147,9162,9177},
            {9192,9207,9222,9236,9251,9265,9278,9292,9306,9319},
            {9332,9345,9357,9370,9382,9394,9406,9418,9430,9441},
            {9452,9463,9474,9484,9495,9505,9515,9525,9535,9545},
            {9554,9564,9573,9582,9591,9599,9608,9616,9625,9633},
            {9641,9648,9656,9664,9671,9678,9686,9693,9700,9706},
            {9713,9719,9726,9732,9738,9744,9750,9756,9762,9767},
            {9772,9778,9783,9788,9793,9798,9803,9808,9812,9817},
            {9821,9826,9830,9834,9838,9842,9846,9850,9854,9857},
            {9861,9864,9868,9871,9874,9878,9881,9884,9887,9890},
            {9893,9896,9898,9901,9904,9906,9909,9911,9913,9916},
            {9918,9920,9922,9925,9927,9929,9931,9932,9934,9936}
        };
        Random random = new Random();
        public int NormalRandom2(int min, int max)
        {
            int result = 0;
            bool flag = true;
            while (flag)
            {

                int x = random.Next(0, 250);
                int y = 10000 - Map[x / 10, x % 10];
                if (y >= (int)random.Next(0, 10000))
                {
                    result = x;
                    flag = false;
                }
            }
            if ((int)random.Next(0, 2) == 1)
            {
                result = (min + max) / 2 - (result + (int)random.Next(0, 2)) * (max - min) / 2 / 250;
            }
            else
            {
                result = (min + max) / 2 + (result + (int)random.Next(0, 2)) * (max - min) / 2 / 250;
            }
            return result;
        }
        #endregion
    }
}
