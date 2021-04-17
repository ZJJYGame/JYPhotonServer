using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using RedisDotNet;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    enum RoleStatusType
    {
        RoleHP=1,
        RoleMP = 2,
        RoleSoul = 3,
        AttackPhysical=4,
        DefendPhysical=5,
        AttackPower=6,
        DefendPower = 7,
        AttackSpeed=8,
        PhysicalCritProb=9,
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP
            //RoleHP






















    }

    public partial class PracticeManager
    {
        /// <summary>
        /// 计算人物属性
        /// </summary>
        public async void RoleStatusAlgorithm( int roleid)
        {
            List<int> gongfaList = new List<int>();
            List<int> mishuList = new List<int>();

            RoleStatusDTO roleStatus = new RoleStatusDTO();
            RoleStatusDTO roleStatusGF = new RoleStatusDTO();
            RoleStatusDTO roleStatusMS = new RoleStatusDTO();

            GameEntry. DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfa);
            GameEntry.DataManager.TryGetValue<Dictionary<int, MiShuData>>(out var mishu);
            GameEntry.DataManager.TryGetValue<Dictionary<int, FlyMagicToolData>>(out var flytool);
            GameEntry.DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleData);

            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
            var rolestatusExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix,roleid.ToString()).Result;
            var roleAptitudeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleid.ToString()).Result;
            var roleMishuExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString()).Result;
            var rolegongfaExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, roleid.ToString()).Result;
            var roleEquipExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleEquipmentPerfix, roleid.ToString()).Result;
            var roleWeaponExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;
            var roleAbilityExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAbilityPointPostfix, roleid.ToString()).Result;
            var roleAllianceSkillExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAllianceSkillPerfix, roleid.ToString()).Result;
            var roleFlyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleFlyMagicToolPerfix, roleid.ToString()).Result;
            if (roleFlyExist&& rolestatusExist && roleMishuExist && rolegongfaExist && roleEquipExist && roleWeaponExist&& roleAbilityExist&& roleAllianceSkillExist&& roleExist)
            {
                var rolestatus = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, roleid.ToString()).Result;
                var roleMishu = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString()).Result;
                var rolegongfa = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleid.ToString()).Result;
                var roleEquip= RedisHelper.Hash.HashGetAsync<RoleEquipmentDTO>(RedisKeyDefine._RoleEquipmentPerfix, roleid.ToString()).Result;
                var roleWeapon = RedisHelper.Hash.HashGetAsync<RoleWeaponDTO>(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;
                var roleAbility= RedisHelper.Hash.HashGetAsync<RoleStatusPointDTO>(RedisKeyDefine._RoleAbilityPointPostfix, roleid.ToString()).Result;
                var roleAllianceSkill= RedisHelper.Hash.HashGetAsync<RoleAllianceSkillDTO>(RedisKeyDefine._RoleAllianceSkillPerfix, roleid.ToString()).Result;
                var roleFly = RedisHelper.Hash.HashGetAsync<FlyMagicToolDTO>(RedisKeyDefine._RoleFlyMagicToolPerfix, roleid.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleDTO>(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
                if (rolestatus!=null && roleMishu!=null && rolegongfa!=null && roleEquip!=null && roleWeapon!=null && roleAbility!=null && roleAllianceSkill!=null && roleFly!=null&& role!=null)
                {
                  var result=  roleData.TryGetValue(role.RoleLevel,out var roleObj);
                    flytool.TryGetValue(roleFly.FlyMagicToolID,out var flyMagicToolData);
                    if (result)
                    {
                        roleStatus.AttackPhysical += roleObj.AttackPhysical + flyMagicToolData.AddPhysicAttack;
                        roleStatus.AttackPower += roleObj.AttackPower + flyMagicToolData.AddMagicAttack;
                        roleStatus.AttackSpeed += roleObj.AttackSpeed ;
                        roleStatus.BestBlood += (short)roleObj.BestBlood;
                        roleStatus.DefendPhysical += roleObj.DefendPhysical;
                        roleStatus.DefendPower += roleObj.DefendPower;
                        roleStatus.GongfaLearnSpeed += roleObj.GongfaLearnSpeed;
                        roleStatus.MagicCritDamage += roleObj.MagicCritDamage;
                        roleStatus.MagicCritProb += roleObj.MagicCritProb;
                        roleStatus.MishuLearnSpeed += roleObj.MishuLearnSpeed;
                        roleStatus.MoveSpeed += roleObj.MoveSpeed+ flyMagicToolData.AddMoveSpeed;
                        roleStatus.PhysicalCritDamage += roleObj.PhysicalCritDamage;
                        roleStatus.PhysicalCritProb += roleObj.PhysicalCritProb;
                        roleStatus.ReduceCritDamage += roleObj.ReduceCritDamage;
                        roleStatus.ReduceCritProb += roleObj.ReduceCritProb;
                        roleStatus.RoleHP += roleObj.RoleHP+ flyMagicToolData.AddRoleHp;
                        roleStatus.RoleMP += roleObj.RoleMP;
                        roleStatus.RolePopularity += roleObj.RolePopularity;
                        roleStatus.RoleSoul += roleObj.RoleSoul;
                        roleStatus.ValueHide += roleObj.ValueHide;
                        roleStatus.Vitality += roleObj.Vitality;
                        roleStatus.MaxVitality += roleStatus.Vitality;
                        roleStatus.RoleMaxMP += roleStatus.RoleMP;
                        roleStatus.RoleMaxHP += roleStatus.RoleHP;
                        roleStatus.RoleSoul += roleStatus.RoleMaxSoul;
                        roleStatus.BestBlood += roleStatus.BestBloodMax;
                    }

                    gongfaList = rolegongfa.GongFaIDArray.Values.ToList();
                    if (gongfaList.Count > 0)
                    {
                        for (int i = 0; i < gongfaList.Count; i++)
                        {
                            roleStatusGF.AttackPhysical += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.AttackPower += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.AttackSpeed += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.BestBlood += (short)gongfa[gongfaList[i]].Best_Blood;
                            roleStatusGF.DefendPhysical += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.DefendPower += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.GongfaLearnSpeed += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.MagicCritDamage += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.MagicCritProb += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.MaxVitality += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.MishuLearnSpeed += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.MoveSpeed += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.PhysicalCritDamage += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.PhysicalCritProb += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.ReduceCritDamage += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.ReduceCritProb += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.RoleHP += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.RoleMP += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.RolePopularity += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.RoleSoul += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.ValueHide += gongfa[gongfaList[i]].Attact_Physical;
                            roleStatusGF.Vitality += gongfa[gongfaList[i]].Attact_Physical;

                            roleStatusGF.RoleMaxHP = roleStatusGF.RoleHP;
                            roleStatusGF.RoleMaxMP = roleStatusGF.RoleMP;
                            roleStatusGF.RoleMaxPopularity = roleStatusGF.RolePopularity;
                            roleStatusGF.RoleMaxSoul = roleStatusGF.RoleSoul;
                            roleStatusGF.BestBloodMax = roleStatusGF.BestBlood;
                        }
                    }

                    if (mishuList.Count > 0)
                    {
                        for (int j = 0; j < mishuList.Count; j++)
                        {
                            roleStatusMS.AttackPhysical += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.AttackPower += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.AttackSpeed += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.BestBlood += (short)gongfa[gongfaList[j]].Best_Blood;
                            roleStatusMS.DefendPhysical += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.DefendPower += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.GongfaLearnSpeed += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.MagicCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.MagicCritProb += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.MaxVitality += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.MishuLearnSpeed += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.MoveSpeed += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.PhysicalCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.PhysicalCritProb += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.ReduceCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.ReduceCritProb += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.RoleHP += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.RoleMP += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.RolePopularity += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.RoleSoul += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.ValueHide += gongfa[gongfaList[j]].Attact_Physical;
                            roleStatusMS.Vitality += gongfa[gongfaList[j]].Attact_Physical;

                            roleStatusMS.RoleMaxHP = roleStatusMS.RoleHP;
                            roleStatusMS.RoleMaxMP = roleStatusMS.RoleMP;
                            roleStatusMS.RoleMaxPopularity = roleStatusMS.RolePopularity;
                            roleStatusMS.RoleMaxSoul = roleStatusMS.RoleSoul;
                            roleStatusMS.BestBloodMax = roleStatusMS.BestBlood;
                        }
                    }

                    foreach (var item in roleEquip.Weapon)
                    {
                        if (roleWeapon.WeaponStatusDict.TryGetValue(item.Value,out var weaponDTO))
                        {
                            //weaponDTO.WeaponAttribute[0]
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算技能加成
        /// </summary>
        public void SkillAlgorithm(List<int>ids)
        {
            
        }
        /// <summary>
        /// 武器技能加成
        /// </summary>
        public void WeaponSkillAlgorithm(List<int> ids,RoleWeaponDTO weapon,RoleEquipmentDTO  roleEquipment)
        {

        }

        /// <summary>
        /// 获取Redis各部分加成
        /// </summary>
        /// <param name="statusFly">飞行法器加成</param>
        /// <param name="statusGF">功法加成</param>
        /// <param name="statusMS">秘术加成</param>
        /// <param name="statusPoint">加点加成</param>
        /// <param name="statusEquip">装备加成</param>
        public async void RoleStatusAlgorithm(RoleStatusDTO statusFly, RoleStatusAdditionDTO statusGF, RoleStatusAdditionDTO statusMS, RoleStatusDTO statusPoint,RoleStatusAdditionDTO statusEquip)
        {
            RoleStatusDTO roleStatus = new RoleStatusDTO();
            roleStatus.AttackPhysical += statusFly.AttackPhysical + statusGF.AttackPhysical+statusEquip.AttackPhysical+statusMS.AttackPhysical+statusPoint.AttackPhysical;
            roleStatus.AttackPower += statusFly.AttackPower + statusGF.AttackPower + statusEquip.AttackPower + statusMS.AttackPower + statusPoint.AttackPower;
            roleStatus.AttackSpeed += statusFly.AttackSpeed + statusGF.AttackSpeed + statusEquip.AttackSpeed + statusMS.AttackSpeed + statusPoint.AttackSpeed;
            roleStatus.BestBlood += (short)(statusFly.BestBlood + statusGF.BestBlood + statusEquip.BestBlood + statusMS.BestBlood + statusPoint.BestBlood);
            roleStatus.DefendPhysical += statusFly.DefendPhysical + statusGF.DefendPhysical + statusEquip.DefendPhysical + statusMS.DefendPhysical + statusPoint.DefendPhysical;
            roleStatus.DefendPower += statusFly.DefendPower + statusGF.DefendPower + statusEquip.DefendPower + statusMS.DefendPower + statusPoint.DefendPower;
            roleStatus.GongfaLearnSpeed += statusFly.GongfaLearnSpeed + statusGF.GongfaLearnSpeed + statusEquip.GongfaLearnSpeed + statusMS.GongfaLearnSpeed + statusPoint.GongfaLearnSpeed;
            roleStatus.MagicCritDamage += statusFly.MagicCritDamage + statusGF.MagicCritDamage + statusEquip.MagicCritDamage + statusMS.MagicCritDamage + statusPoint.MagicCritDamage;
            roleStatus.MagicCritProb += statusFly.MagicCritProb + statusGF.MagicCritProb + statusEquip.MagicCritProb + statusMS.MagicCritProb + statusPoint.MagicCritProb;
            roleStatus.MishuLearnSpeed += statusFly.MishuLearnSpeed + statusGF.MishuLearnSpeed + statusEquip.MishuLearnSpeed + statusMS.MishuLearnSpeed + statusPoint.MishuLearnSpeed;
            roleStatus.MoveSpeed += statusFly.MoveSpeed + statusGF.MoveSpeed + statusEquip.MoveSpeed + statusMS.MoveSpeed + statusPoint.MoveSpeed;
            roleStatus.PhysicalCritDamage += statusFly.PhysicalCritDamage + statusGF.PhysicalCritDamage + statusEquip.PhysicalCritDamage + statusMS.PhysicalCritDamage + statusPoint.PhysicalCritDamage;
            roleStatus.PhysicalCritProb += statusFly.PhysicalCritProb + statusGF.PhysicalCritProb + statusEquip.PhysicalCritProb + statusMS.PhysicalCritProb + statusPoint.PhysicalCritProb;
            roleStatus.ReduceCritDamage += statusFly.ReduceCritDamage + statusGF.ReduceCritDamage + statusEquip.ReduceCritDamage + statusMS.ReduceCritDamage + statusPoint.ReduceCritDamage;
            roleStatus.ReduceCritProb += statusFly.ReduceCritProb + statusGF.ReduceCritProb + statusEquip.ReduceCritProb + statusMS.ReduceCritProb + statusPoint.ReduceCritProb;
            roleStatus.RoleHP += statusFly.RoleHP + statusGF.RoleHP + statusEquip.RoleHP + statusMS.RoleHP + statusPoint.RoleHP;
            roleStatus.RolePopularity += statusFly.RolePopularity + statusGF.RolePopularity + statusEquip.RolePopularity + statusMS.RolePopularity + statusPoint.RolePopularity;
            roleStatus.RoleSoul += statusFly.RoleSoul + statusGF.RoleSoul + statusEquip.RoleSoul + statusMS.RoleSoul + statusPoint.RoleSoul;
            roleStatus.ValueHide += statusFly.ValueHide + statusGF.ValueHide + statusEquip.ValueHide + statusMS.ValueHide + statusPoint.ValueHide;
            roleStatus.Vitality += statusFly.Vitality + statusGF.Vitality + statusEquip.Vitality + statusMS.Vitality + statusPoint.Vitality;






















            roleStatus.MaxVitality += roleStatus.Vitality;
            roleStatus.RoleMaxMP += roleStatus.RoleMP;
            roleStatus.RoleMaxHP += roleStatus.RoleHP;
            roleStatus.RoleSoul += roleStatus.RoleMaxSoul;
            roleStatus.BestBlood += roleStatus.BestBloodMax;
        }
    }
}
