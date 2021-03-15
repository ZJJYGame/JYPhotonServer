using AscensionProtocol.DTO;
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
        public  void CalculateRoleStatus(List<GongFa> gongfaList, List<MishuSkillData> mishuList, RoleStatusDatas roleStatusDatas, out RoleStatus roleStatus)
        {
            var roleStatustemp = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            roleStatustemp.Clear();
            roleStatus = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            if (gongfaList.Count > 0)
            {
                for (int i = 0; i < gongfaList.Count; i++)
                {

                }
            }
            if (mishuList.Count > 0)
            {
                for (int i = 0; i < mishuList.Count; i++)
                {

                }
            }
            OutRolestatus(roleStatus, roleStatusDatas, roleStatustemp, out var tempstatus);
            roleStatus = tempstatus;
            // CosmosEntry.ReferencePoolManager.Despawns(roleStatustemp, roleStatus);
        }

        public  void OutRolestatus(RoleStatus roleStatus, RoleStatusDatas roleStatusDatas, RoleStatus roleStatusTemp, out RoleStatus tempstatus)
        {
            tempstatus = new RoleStatus();
           

        }

        /// <summary>
        ///获取数据库功法秘术数值
        /// </summary>
        /// <param name="gongfaList"></param>
        /// <param name="mishuList"></param>
        /// <param name="roleStatusDatas"></param>
        public void GetRoleStatus(int roleID,RoleStatusDTO roleStatusDTO)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, List<MishuSkillData>>>(out var mishuDict);

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
                    roleStatusDTO.AttackPhysical *= gongfaDict[gongfaList[i]].Attact_Physical/100;
                    roleStatusDTO.AttackPower *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.AttackSpeed *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.BestBlood = (short)(gongfaDict[gongfaList[i]].Attact_Physical * roleStatusDTO.BestBlood / 100);
                    roleStatusDTO.DefendPhysical *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.DefendPower *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.GongfaLearnSpeed *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.MagicCritDamage *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.MagicCritProb *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.MaxVitality *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.MishuLearnSpeed *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.MoveSpeed *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.PhysicalCritDamage *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.PhysicalCritProb *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.ReduceCritDamage *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.ReduceCritProb *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.RoleHP *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.RoleMP *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.RolePopularity *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.RoleSoul *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.ValueHide *= gongfaDict[gongfaList[i]].Attact_Physical / 100;
                    roleStatusDTO.Vitality *= gongfaDict[gongfaList[i]].Attact_Physical / 100;

                    roleStatusDTO.RoleMaxHP = roleStatusDTO.RoleHP;
                    roleStatusDTO.RoleMaxMP = roleStatusDTO.RoleMP;
                    roleStatusDTO.RoleMaxPopularity = roleStatusDTO.RolePopularity;
                    roleStatusDTO.RoleMaxSoul = roleStatusDTO.RoleSoul;
                    roleStatusDTO.BestBloodMax = roleStatusDTO.BestBlood;
                }
            }
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
