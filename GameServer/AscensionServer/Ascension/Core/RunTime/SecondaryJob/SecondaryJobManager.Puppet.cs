﻿using System;
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
       /// <summary>
       /// 合成傀儡部件
       /// </summary>
       /// <param name="roleid"></param>
       /// <param name="useItemID"></param>
      async  void CompoundPuppetS2C(int roleID, int useItemID)
        {
            var puppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetPerfix, roleID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            var assestExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            var puppetUnitExist= RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var rolering = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (puppetExist && roleExist && assestExist&& puppetUnitExist)
            {
                var puppet = RedisHelper.Hash.HashGetAsync<PuppetDTO>(RedisKeyDefine._PuppetPerfix, roleID.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
                var assest = RedisHelper.Hash.HashGetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                var puppetUnit = RedisHelper.Hash.HashGetAsync<PuppetUnitDTO>(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
                if (puppet != null && role != null && assest != null && puppetUnit!=null)
                {
                    var unitid = 0;//部件唯一id
                    GameEntry.DataManager.TryGetValue<Dictionary<int, FormulaPuppetData>>(out var formulaDataDict);//TODO跟换数据类型
                    if (puppet.Recipe_Array.Contains(useItemID))
                    {
                        formulaDataDict.TryGetValue(useItemID, out var formulaData);
                        for (int i = 0; i < formulaData.NeedItemArray.Count; i++)
                        {
                            if (!InventoryManager.VerifyIsExist(formulaData.NeedItemArray[i], formulaData.NeedItemNumber[i], rolering.RingIdArray))
                            {
                                Utility.Debug.LogInfo("YZQ收到的副职业请求1");
                                 RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundPuppet);
                                return;
                            }
                            //if (formulaData.NeedMoney > assest.SpiritStonesLow || formulaData.NeedVitality > role.Vitality)
                            //{
                            //    Utility.Debug.LogInfo("YZQ收到的副职人物属性"+Utility.Json.ToJson(role));
                            //    Utility.Debug.LogInfo("YZQ收到的副职业请求2灵石"+ assest.SpiritStonesLow+"活力"+ role.Vitality);
                            //    RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundPuppet);
                            //    return;
                            //}
                            if (Utility.Algorithm.CreateRandomInt(0, 100) > 100/*formulaData.SuccessRate*/)
                            {
                                Utility.Debug.LogInfo("YZQ收到的副职业请求3");
                                RoleStatusCompoundFailS2C(roleID, SecondaryJobOpCode.CompoundPuppet, default);
                                //鍛造失敗
                                return;
                            }
                            puppet.JobLevelExp += formulaData.MasteryValue;
                            role.Vitality -= formulaData.NeedVitality;
                            assest.SpiritStonesLow -= formulaData.NeedMoney;

                            var puppetObj = PuppetStatusAlgorithm(formulaData.ItemID);
                            if (puppetObj != null)
                            {
                                puppetObj.UnitLevel = formulaData.FormulaLevel;
                                puppetObj.UnitPart = formulaData.SyntheticType;
                                var indexExist = puppetUnit.UnitIndesDict.TryGetValue(formulaData.ItemID, out int id);
                                if (indexExist)
                                {                               
                                    Utility.Debug.LogInfo("YZQ收到的副职业请求的唯一ID为" + unitid);
                                    puppetUnit.UnitIndesDict[formulaData.ItemID] = id + 1;
                                    unitid = Convert.ToInt32(formulaData.ItemID + "" + (id + 1));
                                    puppetUnit.PuppetUnitInfoDict.Add(unitid, puppetObj);
   
                                }
                                else
                                {
                                    puppetUnit.UnitIndesDict.Add(formulaData.ItemID, 1);
                                    unitid = Convert.ToInt32(formulaData.ItemID + "" + 1);
                                    puppetUnit.PuppetUnitInfoDict.Add(unitid, puppetObj);

                                }
                                //TODO发送客户端
                                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                                dict.Add((byte)ParameterCode.GetPuppetUnit, puppetUnit);
                                dict.Add((byte)ParameterCode.RoleAssets, assest);
                                dict.Add((byte)ParameterCode.RoleStatus, role);
                                dict.Add((byte)ParameterCode.JobPuppet, puppet);
                                RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.CompoundPuppet, dict);
                                InventoryManager.AddNewItem(roleID, unitid, 1);

                                #region 更新到数据库
                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString(), puppetUnit);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(puppetUnit));

                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString(), assest);
                                await NHibernateQuerier.UpdateAsync(assest);

                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString(), role);
                                await NHibernateQuerier.UpdateAsync(role);

                                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._PuppetPerfix, roleID.ToString(), puppet);
                                await NHibernateQuerier.UpdateAsync(ChangeDataType(puppet));
                                #endregion
                            }
                            else
                            {
                                Utility.Debug.LogInfo("YZQ收到的副职业请求4");
                                RoleStatusFailS2C(roleID, SecondaryJobOpCode.CompoundPuppet);
                                return;
                            }
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 组装傀儡
        /// </summary>
        async  void AssemblePuppetS2C(int roleID, int useItemID,List<int > unit)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PuppetAssembleData>>(out var unitparameter);
            var result = unitparameter.TryGetValue(useItemID,out var data);
            if (!result)
            {
                return;
            }
            var puppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetPerfix, roleID.ToString()).Result;
            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
            var assestExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
            var puppetUnitExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
            var rolepuppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePuppetPerfix, roleID.ToString()).Result;
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var rolering = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            if (puppetExist&& roleExist&& assestExist&& puppetUnitExist&& rolepuppetExist)
            {
                var puppet = RedisHelper.Hash.HashGetAsync<PuppetDTO>(RedisKeyDefine._PuppetPerfix, roleID.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString()).Result;
                var assest = RedisHelper.Hash.HashGetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString()).Result;
                var puppetUnit = RedisHelper.Hash.HashGetAsync<PuppetUnitDTO>(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
                var rolepuppet = RedisHelper.Hash.HashGetAsync<RolePuppetDTO>(RedisKeyDefine._RolePuppetPerfix,roleID.ToString()).Result;
                if (puppet!=null&& role!=null&& assest!=null&& puppetUnit!=null&& rolepuppet!=null)
                {
                    for (int i = 0; i < unit.Count; i++)
                    {
                        if (!InventoryManager.VerifyIsExist(unit[i], 1, rolering.RingIdArray))
                        {
                            Utility.Debug.LogInfo("YZQ收到的副职业请求1");
                            RoleStatusFailS2C(roleID, SecondaryJobOpCode.AssemblePuppet);
                            return;
                        }
                        if (data.NeedMoney > assest.SpiritStonesLow || data.NeedVitality > role.Vitality)
                        {
                            Utility.Debug.LogInfo("YZQ收到的副职人物属性" + Utility.Json.ToJson(role));
                            Utility.Debug.LogInfo("YZQ收到的副职业请求2灵石" + assest.SpiritStonesLow + "活力" + role.Vitality);
                            RoleStatusFailS2C(roleID, SecondaryJobOpCode.AssemblePuppet);
                            return;
                        }
                    }
                    var puppetObj = PuppetIndividualAlgorithm(puppetUnit, unit);
                    if (puppetObj != null)
                    {
                        role.Vitality -= data.NeedVitality;
                        assest.SpiritStonesLow -= data.NeedMoney;


                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.RoleAssets, assest);
                        dict.Add((byte)ParameterCode.RoleStatus, role);
                        dict.Add((byte)ParameterCode.JobPuppet, puppet);
                        dict.Add((byte)ParameterCode.GetPuppetIndividual, puppet);
                        RoleStatusSuccessS2C(roleID, SecondaryJobOpCode.AssemblePuppet, dict);

                        #region 更新到数据库
                        await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString(), puppetUnit);
                        await NHibernateQuerier.UpdateAsync(ChangeDataType(puppetUnit));

                        await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleAssetsPerfix, roleID.ToString(), assest);
                        await NHibernateQuerier.UpdateAsync(assest);

                        await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatsuPerfix, roleID.ToString(), role);
                        await NHibernateQuerier.UpdateAsync(role);
     
                        #endregion



                    }
                    else
                        RoleStatusFailS2C(roleID, SecondaryJobOpCode.AssemblePuppet);
                }
            }
        }

        void GetPuppetUnitS2C(int roleID)
        {
            var puppetUnitExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
            if (puppetUnitExist)
            {
                var puppetUnit = RedisHelper.Hash.HashGetAsync<PuppetUnitDTO>(RedisKeyDefine._PuppetUnitPerfix, roleID.ToString()).Result;
                if (puppetUnit!=null)
                {
                    RoleStatusSuccessS2C(roleID,SecondaryJobOpCode.GetPuppetUnit, puppetUnit);
                }
            }
        }

        /// <summary>
        /// 傀儡部件属性值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PuppetUnitInfo PuppetStatusAlgorithm(int id)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PuppetUnitParameter>>(out var unitparameter);
            var result = unitparameter.TryGetValue(id, out var parameter);
            if (result)
            {
                PuppetUnitInfo puppet = new PuppetUnitInfo();
                for (int i = 0; i < parameter.PuppetAttributeMax.Count; i++)
                {
                    puppet.PuppetAttribute.Add(Utility.Algorithm.CreateRandomInt(parameter.PuppetAttributeMin[i], parameter.PuppetAttributeMax[i]));
                }
                puppet.WeaponSkill.Add(parameter.FixedSkill);
                if (Utility.Algorithm.CreateRandomInt(0,100)<= parameter.PuppetSkillProbability)
                {
                    puppet.WeaponSkill.Add(parameter.PuppetSkillpoor[Utility.Algorithm.CreateRandomInt(0, parameter.PuppetSkillpoor.Count)]);
                }

                if (Utility.Algorithm.CreateRandomInt(0, 100)<= parameter.AdditionalAttributeProbability)
                {
                    puppet.AffixType = Utility.Algorithm.CreateRandomInt(0, parameter.AdditionalMaxPercentage.Count );
                    puppet.AffixAddition = Utility.Algorithm.CreateRandomInt(0, parameter.AdditionalMaxPercentage[puppet.AffixType] );
                }
                puppet.PuppetDurable = parameter.PuppetDurable;
                return puppet;
            }
            else
                return null;

        }

        /// <summary>
        /// 傀儡组装属性值
        /// </summary>
        /// <returns></returns>
        PuppetIndividualDTO PuppetIndividualAlgorithm(PuppetUnitDTO unitDTO, List<int> unit)
        {
            PuppetIndividualDTO individualDTO = new PuppetIndividualDTO();
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < unit.Count; i++)
            {
              var result=  unitDTO.PuppetUnitInfoDict.TryGetValue(unit[i],out var unitInfo);
                if (!result)
                {
                    return null;
                }
                individualDTO.HP += unitInfo.PuppetAttribute[0];
                individualDTO.AttackPhysical += unitInfo.PuppetAttribute[1];
                individualDTO.AttackPower += unitInfo.PuppetAttribute[2];
                individualDTO.DefendPhysical += unitInfo.PuppetAttribute[3];
                individualDTO.DefendPower += unitInfo.PuppetAttribute[4];
                individualDTO.AttackSpeed += unitInfo.PuppetAttribute[5];
                individualDTO.MP += unitInfo.PuppetAttribute[6];

                individualDTO.PuppetDurable += unitInfo.PuppetDurable;

                for (int j = 0; j < unitInfo.WeaponSkill.Count; j++)
                {
                    if (!individualDTO.Skills.Contains(unitInfo.WeaponSkill[j]))
                    {
                        individualDTO.Skills.Add(unitInfo.WeaponSkill[j]);
                    }
                }

                if (dict.ContainsKey(unitInfo.AffixType))
                {
                    dict[unitInfo.AffixType] += unitInfo.AffixAddition;
                }
                else
                {
                    dict.Add(unitInfo.AffixType, unitInfo.AffixAddition);
                }

                #region
                foreach (var item in dict)
                {
                    switch ((AffixType)item.Key)
                    {
                        case AffixType.HP:
                            individualDTO.HP *= (item.Value + 100) / 100;
                            break;
                        case AffixType.AttackPhysical:
                            individualDTO.AttackPhysical *= (100 + item.Value) / 100;
                            break;
                        case AffixType.DefendPhysical:
                            individualDTO.AttackPower *= (100 + item.Value) / 100;
                            break;
                        case AffixType.AttackPower:
                            individualDTO.DefendPhysical *= (100 + item.Value) / 100;
                            break;
                        case AffixType.DefendPower:
                            individualDTO.DefendPower *= (100 + item.Value) / 100;
                            break;
                        case AffixType.AttackSpeed:
                            individualDTO.AttackSpeed *= (100 + item.Value) / 100;
                            break;
                        default:
                            break;
                    }
                }

                #endregion

            }
            return individualDTO;
        }

        PuppetDTO ChangeDataType(Puppet puppet)
        {
            PuppetDTO puppetDTO = new PuppetDTO();
            puppetDTO.RoleID = puppet.RoleID;
            puppetDTO.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(puppet.Recipe_Array);
            puppetDTO.JobLevel = puppet.JobLevel;
            puppetDTO.JobLevelExp = puppet.JobLevelExp;
            return puppetDTO;
        }

        Puppet ChangeDataType(PuppetDTO puppetDTO)
        {
            Puppet puppet = new Puppet();
            puppet.RoleID = puppet.RoleID;
            puppet.Recipe_Array = Utility.Json.ToJson(puppet.Recipe_Array);
            puppet.JobLevel = puppet.JobLevel;
            puppet.JobLevelExp = puppet.JobLevelExp;
            return puppet;
        }

        PuppetUnit ChangeDataType(PuppetUnitDTO unitDTO)
        {
            PuppetUnit puppet = new PuppetUnit();
            puppet.RoleID = unitDTO.RoleID;
            puppet.PuppetUnitInfoDict = Utility.Json.ToJson(unitDTO.PuppetUnitInfoDict);
            puppet.UnitIndesDict = Utility.Json.ToJson(unitDTO.UnitIndesDict);
            return puppet;
        }
    }
}
