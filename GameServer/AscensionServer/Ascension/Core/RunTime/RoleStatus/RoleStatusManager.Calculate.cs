﻿using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;
namespace AscensionServer
{
   public partial class RoleStatusManager
    {
        public  void CalculateRoleStatus()
        {

        }

        /// <summary>
        ///获取数据库功法秘术数值
        /// </summary>
        /// <param name="gongfaList"></param>
        /// <param name="mishuList"></param>
        /// <param name="roleStatusDatas"></param>
        public RoleStatusDTO GetRoleStatus(int roleID,RoleStatusDatas roleStatus)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, List<MishuSkillData>>>(out var mishuDict);

            RoleStatusDTO roleStatusGF = new RoleStatusDTO();
            RoleStatusDTO roleStatusmishu = new RoleStatusDTO();

            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfaDict);
            List<int> gongfaList = new List<int>();
            List<MishuSkillData> mishuList = new List<MishuSkillData>();
            #region 获取角色所有功法
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString()).Result)
            {
                var roleGongfa = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleID.ToString()).Result;
                if (roleGongfa != null)
                {
                    gongfaList = roleGongfa.GongFaIDArray.Values.ToList();
                }
                else
                {
                    NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var rolegongfa = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRole);
                    if (rolegongfa != null)
                    {
                        var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolegongfa.GongFaIDArray);
                        gongfaList = dict.Values.ToList();
                    }
                }

            }
            else
            {
                NHCriteria nHCriteriaRole= CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                var rolegongfa = NHibernateQuerier.CriteriaSelect<RoleGongFa>(nHCriteriaRole);
                if (rolegongfa!=null)
                {
                   var dict= Utility.Json.ToObject<Dictionary<int,int>>(rolegongfa.GongFaIDArray);
                    gongfaList = dict.Values.ToList();
                }
            }
            #endregion

            #region 获取角色所有功法
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString()).Result)
            {
                var roleMiShu = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleID.ToString()).Result;
                if (roleMiShu != null)
                {
                    foreach (var item in roleMiShu.MiShuIDArray)
                    {
                        NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                        var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                        if (temp != null)
                            mishuList.Add(temp);
                    }
                }
                else
                {
                    NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                    var rolemishu = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRole);
                    if (rolemishu != null)
                    {
                        var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolemishu.MiShuIDArray);
                        foreach (var item in dict)
                        {
                            NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                            var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                            var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                            if (temp != null)
                                mishuList.Add(temp);
                        }
                    }
                }

            }
            else
            {
                NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
                var rolemishu = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriaRole);
                if (rolemishu != null)
                {
                    var dict = Utility.Json.ToObject<Dictionary<int, int>>(rolemishu.MiShuIDArray);
                    foreach (var item in dict)
                    {
                        NHCriteria nHCriteriamishu = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                        var mishuObj = NHibernateQuerier.CriteriaSelect<MiShu>(nHCriteriamishu);
                        var temp = mishuDict[item.Value].Find(t => t.Mishu_Floor == mishuObj.MiShuLevel);
                        if (temp != null)
                            mishuList.Add(temp);
                    }
                }
            }

            #endregion

            if (gongfaList.Count>0)
            {
                for (int i = 0; i < gongfaList.Count; i++)
                {
                    roleStatusGF.AttackPhysical += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.AttackPower += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.AttackSpeed += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.BestBlood += (short)gongfaDict[gongfaList[i]].Best_Blood;
                    roleStatusGF.DefendPhysical += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.DefendPower += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.GongfaLearnSpeed += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.MagicCritDamage += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.MagicCritProb += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.MaxVitality += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.MishuLearnSpeed += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.MoveSpeed += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.PhysicalCritDamage += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.PhysicalCritProb += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.ReduceCritDamage += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.ReduceCritProb += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.RoleHP += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.RoleMP += gongfaDict[gongfaList[i]].Attact_Physical;
                    roleStatusGF.RolePopularity += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.RoleSoul += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.ValueHide += gongfaDict[gongfaList[i]].Attact_Physical ;
                    roleStatusGF.Vitality += gongfaDict[gongfaList[i]].Attact_Physical ;

                    roleStatusGF.RoleMaxHP = roleStatusGF.RoleHP;
                    roleStatusGF.RoleMaxMP = roleStatusGF.RoleMP;
                    roleStatusGF.RoleMaxPopularity = roleStatusGF.RolePopularity;
                    roleStatusGF.RoleMaxSoul = roleStatusGF.RoleSoul;
                    roleStatusGF.BestBloodMax = roleStatusGF.BestBlood;
                }
            }

            if (mishuList.Count>0)
            {
                for (int j = 0; j < mishuList.Count; j++)
                {
                    roleStatusmishu.AttackPhysical += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.AttackPower += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.AttackSpeed += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.BestBlood += (short)gongfaDict[gongfaList[j]].Best_Blood;
                    roleStatusmishu.DefendPhysical += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.DefendPower += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.GongfaLearnSpeed += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.MagicCritDamage += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.MagicCritProb += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.MaxVitality += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.MishuLearnSpeed += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.MoveSpeed += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.PhysicalCritDamage += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.PhysicalCritProb += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.ReduceCritDamage += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.ReduceCritProb += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.RoleHP += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.RoleMP += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.RolePopularity += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.RoleSoul += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.ValueHide += gongfaDict[gongfaList[j]].Attact_Physical;
                    roleStatusmishu.Vitality += gongfaDict[gongfaList[j]].Attact_Physical;

                    roleStatusmishu.RoleMaxHP = roleStatusmishu.RoleHP;
                    roleStatusmishu.RoleMaxMP = roleStatusmishu.RoleMP;
                    roleStatusmishu.RoleMaxPopularity = roleStatusmishu.RolePopularity;
                    roleStatusmishu.RoleMaxSoul = roleStatusmishu.RoleSoul;
                    roleStatusmishu.BestBloodMax = roleStatusmishu.BestBlood;
                }
            }

            #region 计算
            roleStatusmishu.AttackPhysical += ((roleStatus.AttackPhysical * roleStatusGF.AttackPhysical / 100) + roleStatus.AttackPhysical);
            roleStatusmishu.AttackPower += ((roleStatus.AttackPower * roleStatusGF.AttackPhysical / 100) + roleStatus.AttackPower);
            roleStatusmishu.AttackSpeed += ((roleStatus.AttackSpeed * roleStatusGF.AttackPhysical / 100) + roleStatus.AttackSpeed);
            roleStatusmishu.BestBlood += (short)((roleStatus.BestBlood * roleStatusGF.AttackPhysical / 100) + roleStatus.BestBlood);
            roleStatusmishu.DefendPhysical += ((roleStatus.DefendPhysical * roleStatusGF.AttackPhysical / 100) + roleStatus.DefendPhysical);
            roleStatusmishu.DefendPower += ((roleStatus.DefendPower * roleStatusGF.AttackPhysical / 100) + roleStatus.DefendPower);
            roleStatusmishu.GongfaLearnSpeed += ((roleStatus.GongfaLearnSpeed * roleStatusGF.AttackPhysical / 100) + roleStatus.GongfaLearnSpeed);
            roleStatusmishu.MagicCritDamage += ((roleStatus.MagicCritDamage * roleStatusGF.AttackPhysical / 100) + roleStatus.MagicCritDamage);
            roleStatusmishu.MagicCritProb += ((roleStatus.MagicCritProb * roleStatusGF.AttackPhysical / 100) + roleStatus.MagicCritProb);
            roleStatusmishu.MishuLearnSpeed += ((roleStatus.MishuLearnSpeed * roleStatusGF.AttackPhysical / 100) + roleStatus.MishuLearnSpeed);
            roleStatusmishu.MoveSpeed += ((roleStatus.MoveSpeed * roleStatusGF.AttackPhysical / 100) + roleStatus.MoveSpeed);
            roleStatusmishu.PhysicalCritDamage += ((roleStatus.PhysicalCritDamage * roleStatusGF.AttackPhysical / 100) + roleStatus.PhysicalCritDamage);
            roleStatusmishu.PhysicalCritProb += ((roleStatus.PhysicalCritProb * roleStatusGF.AttackPhysical / 100) + roleStatus.PhysicalCritProb);
            roleStatusmishu.ReduceCritDamage += ((roleStatus.ReduceCritDamage * roleStatusGF.AttackPhysical / 100) + roleStatus.ReduceCritDamage);
            roleStatusmishu.ReduceCritProb += ((roleStatus.ReduceCritProb * roleStatusGF.AttackPhysical / 100) + roleStatus.ReduceCritProb);
            roleStatusmishu.RoleHP += ((roleStatus.RoleHP * roleStatusGF.AttackPhysical / 100) + roleStatus.RoleHP);
            roleStatusmishu.RoleMP += ((roleStatus.RoleMP * roleStatusGF.AttackPhysical / 100) + roleStatus.RoleMP);
            roleStatusmishu.RolePopularity += ((roleStatus.RolePopularity * roleStatusGF.AttackPhysical / 100) + roleStatus.RolePopularity);
            roleStatusmishu.RoleSoul += ((roleStatus.RoleSoul * roleStatusGF.AttackPhysical / 100) + roleStatus.RoleSoul);
            roleStatusmishu.ValueHide += ((roleStatus.ValueHide * roleStatusGF.AttackPhysical / 100) + roleStatus.ValueHide);
            roleStatusmishu.Vitality += ((roleStatus.Vitality * roleStatusGF.AttackPhysical / 100) + roleStatus.Vitality);
            #endregion
            return roleStatusmishu;
        }
        /// <summary>
        /// 飞行法器数值加上人物加点数值
        /// </summary>
        /// <param name="flyID"></param>
        /// <param name="roleStatusPoint"></param>
        /// <param name="roleStatusDTO"></param>
        /// <returns></returns>
        public RoleStatusDTO SumRolestatus(int flyID,RoleStatusPointDTO roleStatusPoint,RoleStatusDTO roleStatusDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<string, RoleAbilityPointData>>(out var abilityDict);
            GameEntry.DataManager.TryGetValue<Dictionary<int, FlyMagicToolData>>(out var flyDataDict);

           var result= flyDataDict.TryGetValue(flyID,out var flyMagicDict);
            if (result)
            {
                if (flyMagicDict.FixedSpeed!=-1)
                    roleStatusDTO.MoveSpeed = flyMagicDict.FixedSpeed;
                else
                    roleStatusDTO.MoveSpeed += flyMagicDict.AddMoveSpeed;
                roleStatusDTO.AttackPhysical += flyMagicDict.AddPhysicAttack;
                roleStatusDTO.AttackPower += flyMagicDict.AddMagicAttack;
                roleStatusDTO.RoleHP += flyMagicDict.AddRoleHp;
                roleStatusDTO.RoleMaxHP += flyMagicDict.AddRoleHp;
            }

            var pointResult = roleStatusPoint.AbilityPointSln.TryGetValue(roleStatusPoint.SlnNow,out var pointDict);
            if (pointResult)
            {
                roleStatusDTO.RoleHP = pointDict.Corporeity * 10;
                roleStatusDTO.RoleMP = pointDict.Power * 6+ pointDict.Corporeity;
                roleStatusDTO.RoleSoul = pointDict.Soul * 10;
                roleStatusDTO.BestBlood = (short)(pointDict.Corporeity * 0.1f);
                roleStatusDTO.AttackPhysical = pointDict.Strength * 2;
                roleStatusDTO.DefendPhysical = pointDict.Stamina * 2;
                roleStatusDTO.AttackPower = pointDict.Power * 2;
                roleStatusDTO.DefendPower = (int)(pointDict.Strength * 1.2f + pointDict.Power + pointDict.Corporeity * 0.4f + pointDict.Stamina * 0.8f);
                roleStatusDTO.AttackSpeed = (int)(pointDict.Soul * 0.2f + pointDict.Stamina * 0.1f + pointDict.Corporeity * 0.1 + pointDict.Agility * 0.5f + pointDict.Strength * 0.1f);
                roleStatusDTO.MoveSpeed = (int)(pointDict.Agility * 0.1f);
            }

            return roleStatusDTO;
        }
    }
}
