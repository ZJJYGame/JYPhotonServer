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
        PhysicalCritDamage=10,
        MagicCritProb=11,
        MagicCritDamage=12,
        ReduceCritProb=13,
        ReduceCritDamage=14,
        MoveSpeed=15,
        GongfaLearnSpeed=16,
        MishuLearnSpeed=17,
    }

    public partial class PracticeManager
    {
        /// <summary>
        /// 计算人物属性待删
        /// </summary>
        public async void RoleStatusAlgorithm(int roleid)
        {
            List<int> gongfaList = new List<int>();
            List<int> mishuList = new List<int>();

            RoleStatusDTO roleStatus = new RoleStatusDTO();
            RoleStatusDTO roleStatusGF = new RoleStatusDTO();
            RoleStatusDTO roleStatusMS = new RoleStatusDTO();

            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfa);
            GameEntry.DataManager.TryGetValue<Dictionary<int, MiShuData>>(out var mishu);
            GameEntry.DataManager.TryGetValue<Dictionary<int, FlyMagicToolData>>(out var flytool);
            GameEntry.DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleData);

            var roleExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
            var rolestatusExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleid.ToString()).Result;
            var roleAptitudeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatsuPerfix, roleid.ToString()).Result;
            var roleMishuExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString()).Result;
            var rolegongfaExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleGongfaPerfix, roleid.ToString()).Result;
            var roleEquipExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleEquipmentPerfix, roleid.ToString()).Result;
            var roleWeaponExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;
            var roleAbilityExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAbilityPointPostfix, roleid.ToString()).Result;
            var roleAllianceSkillExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAllianceSkillPerfix, roleid.ToString()).Result;
            var roleFlyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleFlyMagicToolPerfix, roleid.ToString()).Result;
            if (roleFlyExist && rolestatusExist && roleMishuExist && rolegongfaExist && roleEquipExist && roleWeaponExist && roleAbilityExist && roleAllianceSkillExist && roleExist)
            {
                var rolestatus = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatsuPerfix, roleid.ToString()).Result;
                var roleMishu = RedisHelper.Hash.HashGetAsync<RoleMiShuDTO>(RedisKeyDefine._RoleMiShuPerfix, roleid.ToString()).Result;
                var rolegongfa = RedisHelper.Hash.HashGetAsync<RoleGongFaDTO>(RedisKeyDefine._RoleGongfaPerfix, roleid.ToString()).Result;
                var roleEquip = RedisHelper.Hash.HashGetAsync<RoleEquipmentDTO>(RedisKeyDefine._RoleEquipmentPerfix, roleid.ToString()).Result;
                var roleWeapon = RedisHelper.Hash.HashGetAsync<RoleWeaponDTO>(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;
                var roleAbility = RedisHelper.Hash.HashGetAsync<RoleStatusPointDTO>(RedisKeyDefine._RoleAbilityPointPostfix, roleid.ToString()).Result;
                var roleAllianceSkill = RedisHelper.Hash.HashGetAsync<RoleAllianceSkillDTO>(RedisKeyDefine._RoleAllianceSkillPerfix, roleid.ToString()).Result;
                var roleFly = RedisHelper.Hash.HashGetAsync<FlyMagicToolDTO>(RedisKeyDefine._RoleFlyMagicToolPerfix, roleid.ToString()).Result;
                var role = RedisHelper.Hash.HashGetAsync<RoleDTO>(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
                #region
                if (rolestatus != null && roleMishu != null && rolegongfa != null && roleEquip != null && roleWeapon != null && roleAbility != null && roleAllianceSkill != null && roleFly != null && role != null)
                {
                    var result = roleData.TryGetValue(role.RoleLevel, out var roleObj);
                    flytool.TryGetValue(roleFly.FlyMagicToolID, out var flyMagicToolData);
                    if (result)
                    {
                        roleStatus.AttackPhysical += roleObj.AttackPhysical + flyMagicToolData.AddPhysicAttack;
                        roleStatus.AttackPower += roleObj.AttackPower + flyMagicToolData.AddMagicAttack;
                        roleStatus.AttackSpeed += roleObj.AttackSpeed;
                        roleStatus.BestBlood += (short)roleObj.BestBlood;
                        roleStatus.DefendPhysical += roleObj.DefendPhysical;
                        roleStatus.DefendPower += roleObj.DefendPower;
                        roleStatus.GongfaLearnSpeed += roleObj.GongfaLearnSpeed;
                        roleStatus.MagicCritDamage += roleObj.MagicCritDamage;
                        roleStatus.MagicCritProb += roleObj.MagicCritProb;
                        roleStatus.MishuLearnSpeed += roleObj.MishuLearnSpeed;
                        roleStatus.MoveSpeed += roleObj.MoveSpeed + flyMagicToolData.AddMoveSpeed;
                        roleStatus.PhysicalCritDamage += roleObj.PhysicalCritDamage;
                        roleStatus.PhysicalCritProb += roleObj.PhysicalCritProb;
                        roleStatus.ReduceCritDamage += roleObj.ReduceCritDamage;
                        roleStatus.ReduceCritProb += roleObj.ReduceCritProb;
                        roleStatus.RoleHP += roleObj.RoleHP + flyMagicToolData.AddRoleHp;
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
                        if (roleWeapon.WeaponStatusDict.TryGetValue(item.Value, out var weaponDTO))
                        {
                            //weaponDTO.WeaponAttribute[0]
                        }
                    }
                }
                #endregion
            }
        }
        /// <summary>
        /// 人物加点的加成
        /// </summary>
        /// <param name="pointDTO"></param>
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        public async Task<RoleStatus>  RoleAblility(RoleStatusPointDTO pointDTO, RoleStatus roleStatus)
        {
            RoleStatusDTO roleStatusDTO = new RoleStatusDTO();
            roleStatusDTO.RoleID = pointDTO.RoleID;
            if (pointDTO.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var abilityDTO))
            {
                roleStatusDTO.RoleMaxHP += abilityDTO.Corporeity * 10;
                roleStatusDTO.RoleMaxMP += (abilityDTO.Power * 6 + abilityDTO.Corporeity);
                roleStatusDTO.RoleMaxSoul += abilityDTO.Soul * 4;
                roleStatusDTO.BestBloodMax += (short)(abilityDTO.Corporeity * 0.1);
                roleStatusDTO.AttackPhysical += abilityDTO.Strength * 2;
                roleStatusDTO.DefendPhysical += abilityDTO.Stamina * 2;
                roleStatusDTO.AttackPower += abilityDTO.Power * 2;
                roleStatusDTO.DefendPower += (int)(abilityDTO.Stamina * 0.8 + abilityDTO.Corporeity * 0.8 + abilityDTO.Power + abilityDTO.Strength * 1.2);
                roleStatusDTO.AttackSpeed += (int)(abilityDTO.Soul * 0.2 + abilityDTO.Stamina * 0.1 + abilityDTO.Corporeity * 0.1 + abilityDTO.Agility * 0.5 + abilityDTO.Strength * 0.1);
                roleStatusDTO.MoveSpeed += (int)(abilityDTO.Agility * 0.1);
            }
            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusAddPointPerfix, pointDTO.RoleID.ToString(), roleStatusDTO);
          var obj=   RoleStatusAlgorithm(pointDTO.RoleID,null,null, null,roleStatusDTO,null);
            if (obj.RoleMaxHP> roleStatus.RoleMaxHP)
            {
                roleStatus.RoleMaxHP = obj.RoleMaxHP;
                roleStatus.RoleHP += (obj.RoleMaxHP - roleStatus.RoleMaxHP);
            }
            if (obj.RoleMaxMP > roleStatus.RoleMaxMP)
            {
                roleStatus.RoleMaxMP = obj.RoleMaxMP;
                roleStatus.RoleMP += (obj.RoleMaxMP - roleStatus.RoleMaxMP);
            }
            if (obj.RoleMaxSoul > roleStatus.RoleMaxSoul)
            {
                roleStatus.RoleMaxSoul = obj.RoleMaxSoul;
                roleStatus.RoleSoul += (obj.RoleMaxSoul - roleStatus.RoleMaxSoul);
            }
            if (obj.BestBloodMax > roleStatus.BestBloodMax)
            {
                roleStatus.BestBloodMax = obj.BestBloodMax;
                roleStatus.BestBlood += (short)(obj.BestBloodMax - roleStatus.BestBloodMax);
            }

            roleStatus.AttackPhysical += roleStatusDTO.AttackPhysical ;
            roleStatus.AttackPower += roleStatusDTO.AttackPower ;
            roleStatus.AttackSpeed += roleStatusDTO.AttackSpeed;
            roleStatus.DefendPhysical += roleStatusDTO.DefendPhysical;
            roleStatus.DefendPower += roleStatusDTO.DefendPower;
            roleStatus.GongfaLearnSpeed += roleStatusDTO.GongfaLearnSpeed;
            roleStatus.MagicCritDamage += roleStatusDTO.MagicCritDamage;
            roleStatus.MagicCritProb += roleStatusDTO.MagicCritProb;
            roleStatus.MishuLearnSpeed += roleStatusDTO.MishuLearnSpeed;
            roleStatus.MoveSpeed += roleStatusDTO.MoveSpeed ;
            roleStatus.PhysicalCritDamage += roleStatusDTO.PhysicalCritDamage;
            roleStatus.PhysicalCritProb += roleStatusDTO.PhysicalCritProb;
            roleStatus.ReduceCritDamage += roleStatusDTO.ReduceCritDamage;
            roleStatus.ReduceCritProb += roleStatusDTO.ReduceCritProb;
            roleStatus.RolePopularity += roleStatusDTO.RolePopularity;
            roleStatus.ValueHide += roleStatusDTO.ValueHide;
            roleStatus.Vitality += roleStatusDTO.Vitality;
            roleStatus.MaxVitality += roleStatusDTO.Vitality;











            return roleStatus;
        }

        #region 角色装备属性计算
        public RoleStatusAdditionDTO Addition<T>(T data, RoleStatusAdditionDTO roleStatus) where T : IPassiveSkills
        {
            for (int j = 0; j < data.Attribute.Count; j++)
            {
                var type = data.Attribute[j];
                var Fixed = data.Fixed;
                var Percentage = data.Percentage;
                roleStatus.Percentage.TryAdd(type, Percentage[j]);
                switch ((RoleStatusType)type)
                {
                    case RoleStatusType.RoleHP:
                        roleStatus.RoleMaxHP += Fixed[type];
                        break;
                    case RoleStatusType.RoleMP:
                        roleStatus.RoleMaxMP += Fixed[type];
                        break;
                    case RoleStatusType.RoleSoul:
                        roleStatus.RoleMaxSoul += Fixed[type];
                        break;
                    case RoleStatusType.AttackPhysical:
                        roleStatus.AttackPhysical += Fixed[type];
                        break;
                    case RoleStatusType.DefendPhysical:
                        roleStatus.DefendPhysical += Fixed[type];
                        break;
                    case RoleStatusType.AttackPower:
                        roleStatus.AttackPower += Fixed[type];
                        break;
                    case RoleStatusType.DefendPower:
                        roleStatus.DefendPower += Fixed[type];
                        break;
                    case RoleStatusType.AttackSpeed:
                        roleStatus.AttackSpeed += Fixed[type];
                        break;
                    case RoleStatusType.PhysicalCritProb:
                        roleStatus.PhysicalCritProb += Fixed[type];
                        break;
                    case RoleStatusType.PhysicalCritDamage:
                        roleStatus.PhysicalCritDamage += Fixed[type];
                        break;
                    case RoleStatusType.MagicCritProb:
                        roleStatus.MagicCritProb += Fixed[type];
                        break;
                    case RoleStatusType.MagicCritDamage:
                        roleStatus.MagicCritDamage += Fixed[type];
                        break;
                    case RoleStatusType.ReduceCritProb:
                        roleStatus.ReduceCritProb += Fixed[type];
                        break;
                    case RoleStatusType.ReduceCritDamage:
                        roleStatus.ReduceCritDamage += Fixed[type];
                        break;
                    case RoleStatusType.MoveSpeed:
                        roleStatus.MoveSpeed += Fixed[type];
                        break;
                    case RoleStatusType.GongfaLearnSpeed:
                        roleStatus.GongfaLearnSpeed += Fixed[type];
                        break;
                    case RoleStatusType.MishuLearnSpeed:
                        roleStatus.MishuLearnSpeed += Fixed[type];
                        break;
                    default:
                        break;
                }
            }

            return roleStatus;
        }

        public WeaponDTO AdditionWeapon(WeaponDTO addDTO, WeaponDTO targetDTO)
        {

            for (int i = 0; i < addDTO.WeaponAttribute.Count; i++)
                targetDTO.WeaponAttribute[i] += addDTO.WeaponAttribute[i];

            return targetDTO;
        }

        public RoleStatusAdditionDTO AdditionWeaponStatus(WeaponDTO weapon, RoleStatusAdditionDTO roleStatus)
        {
            roleStatus.RoleMaxHP += weapon.WeaponAttribute[0];
            roleStatus.AttackPhysical += weapon.WeaponAttribute[1];
            roleStatus.DefendPhysical += weapon.WeaponAttribute[2];
            roleStatus.AttackPower += weapon.WeaponAttribute[3];
            roleStatus.DefendPower += weapon.WeaponAttribute[4];
            roleStatus.AttackSpeed += weapon.WeaponAttribute[5];

            return roleStatus;
        }

        public async Task<RoleStatus> RoleEquip(RoleWeaponDTO roleWeapon, RoleEquipmentDTO roleEquipment)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PassiveSkillsWeapon>>(out var weaponSkillDict);
            RoleStatusAdditionDTO roleStatus = new RoleStatusAdditionDTO();
            WeaponDTO weapon = new WeaponDTO();
            var Weaponlist = new List<WeaponDTO>();

            foreach (var item in roleEquipment.Weapon)
            {
                roleWeapon.WeaponStatusDict.TryGetValue(item.Value, out var weaponDTO);
                Weaponlist.Add(weaponDTO);
            }

            for (int i = 0; i < Weaponlist.Count; i++)
            {
                for (int j = 0; j < Weaponlist[i].WeaponSkill.Count; j++)
                {
                    var skill = weaponSkillDict[Weaponlist[i].WeaponSkill[j]];
                    switch (skill.AddTarget)
                    {
                        case 0:
                            roleStatus = Addition(skill, roleStatus);
                            break;
                        case 1:
                            weapon = AdditionWeapon(Weaponlist[i], weapon);
                            break;
                    }
                }
            }

            roleStatus = AdditionWeaponStatus(weapon, roleStatus);

           var status= RoleStatusAlgorithm(16,null,null,null,null, roleStatus);

            Utility.Debug.LogError("YZQ装备法宝后的属性加成"+Utility.Json.ToJson(status));

            return default;
        }
        #endregion


        /// <summary>
        /// 计算技能加成
        /// </summary>
        public void SkillAlgorithm(List<int> ids)
        {

        }
        /// <summary>
        /// 武器技能加成
        /// </summary>
        public void WeaponSkillAlgorithm(List<int> ids, RoleWeaponDTO weapon, RoleEquipmentDTO roleEquipment)
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
        public  RoleStatusDTO RoleStatusAlgorithm(int roleid=16, RoleStatusDTO statusFly = null, RoleStatusAdditionDTO statusGF = null, RoleStatusAdditionDTO statusMS = null, RoleStatusDTO statusPoint = null, RoleStatusAdditionDTO statusEquip = null)
        {
            if (statusFly == null)
            {
                Utility.Debug.LogError("123");
                var flyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusFlyPerfix, roleid.ToString()).Result;
                if (flyExist)
                {
                    Utility.Debug.LogError("234");
                    statusFly = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatusFlyPerfix, roleid.ToString()).Result;
                    if (statusFly==null)
                    {
                        Utility.Debug.LogError("345");
               
                    }
                }
                else statusFly = new RoleStatusDTO();
            }
            if (statusGF == null)
            {
                var statusGFExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusGFPerfix, roleid.ToString()).Result;
                if (statusGFExist)
                {                  
                    statusGF = RedisHelper.Hash.HashGetAsync<RoleStatusAdditionDTO>(RedisKeyDefine._RoleStatusGFPerfix, roleid.ToString()).Result;
                    if (statusGF==null)
                    {
                        statusGF = new RoleStatusAdditionDTO();
                    }
                }
                else statusGF = new RoleStatusAdditionDTO();
            }
            if (statusMS == null)
            {
                var statusMSExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusMSPerfix, roleid.ToString()).Result;
                if (statusMSExist)
                {
                    statusMS = RedisHelper.Hash.HashGetAsync<RoleStatusAdditionDTO>(RedisKeyDefine._RoleStatusMSPerfix, roleid.ToString()).Result;
                    if (statusMS==null)
                    {
                        statusMS = new RoleStatusAdditionDTO();
                    }
                }
                else statusMS = new RoleStatusAdditionDTO();
            }
            if (statusEquip == null)
            {
                var statusEquipExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusEquipPerfix, roleid.ToString()).Result;
                if (statusEquipExist)
                {
                    statusEquip = RedisHelper.Hash.HashGetAsync<RoleStatusAdditionDTO>(RedisKeyDefine._RoleStatusEquipPerfix, roleid.ToString()).Result;
                    if (statusEquip==null)
                    {
                        statusEquip = new RoleStatusAdditionDTO();
                    }
                }else statusEquip = new RoleStatusAdditionDTO();
            }
            if (statusPoint == null)
            {
                var statusPointExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                if (statusPointExist)
                {
                    statusPoint = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                    if (statusPoint==null)
                    {
                        statusPoint = new RoleStatusDTO();
                    }
                }
                else statusPoint = new RoleStatusDTO();
            }

            RoleStatusType statusType = new RoleStatusType();
            RoleStatusDTO roleStatus = new RoleStatusDTO();
            var Percentage = 0;
            #region 
            roleStatus.AttackPhysical += statusFly.AttackPhysical + statusGF.AttackPhysical + statusEquip.AttackPhysical + statusMS.AttackPhysical + statusPoint.AttackPhysical;
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
            #endregion

            switch (statusType)
            {
                case RoleStatusType.RoleHP:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.RoleHP);
                    roleStatus.RoleMaxHP *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.RoleMP:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.RoleMP);
                    roleStatus.RoleMaxMP *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.RoleSoul:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.RoleSoul);
                    roleStatus.RoleMaxSoul *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.AttackPhysical:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.AttackPhysical);
                    roleStatus.AttackPhysical *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.DefendPhysical:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.DefendPhysical);
                    roleStatus.DefendPhysical *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.AttackPower:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.AttackPower);
                    roleStatus.AttackPower *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.DefendPower:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.DefendPower);
                    roleStatus.DefendPower *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.AttackSpeed:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.AttackSpeed);
                    roleStatus.AttackSpeed *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.PhysicalCritProb:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.PhysicalCritProb);
                    roleStatus.PhysicalCritProb *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.PhysicalCritDamage:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.PhysicalCritDamage);
                    roleStatus.PhysicalCritDamage *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.MagicCritProb:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.MagicCritProb);
                    roleStatus.MagicCritProb *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.MagicCritDamage:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.MagicCritDamage);
                    roleStatus.MagicCritDamage *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.ReduceCritProb:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.ReduceCritProb);
                    roleStatus.ReduceCritProb *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.ReduceCritDamage:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.ReduceCritDamage);
                    roleStatus.ReduceCritDamage *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.MoveSpeed:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.MoveSpeed);
                    roleStatus.MoveSpeed *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.GongfaLearnSpeed:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.GongfaLearnSpeed);
                    roleStatus.GongfaLearnSpeed *= (Percentage + 100) / 100;
                    break;
                case RoleStatusType.MishuLearnSpeed:
                    Percentage = AddPercentage(statusGF, statusMS, statusEquip, RoleStatusType.MishuLearnSpeed);
                    roleStatus.MishuLearnSpeed *= (Percentage + 100) / 100;
                    break;
                default:
                    break;
            }
            Utility.Debug.LogError(Utility.Json.ToJson(roleStatus));

            return roleStatus;
        }

        int AddPercentage(RoleStatusAdditionDTO statusGF, RoleStatusAdditionDTO statusMS, RoleStatusAdditionDTO statusEquip, RoleStatusType statusType)
        {
            int num = 0;
            if (statusGF.Percentage.TryGetValue((int)statusType, out int gf))
            {
                num += gf;
            }
            if (statusMS.Percentage.TryGetValue((int)statusType, out int ms))
            {
                num += ms;
            }
            if (statusEquip.Percentage.TryGetValue((int)statusType, out int equip))
            {
                num += equip;
            }
            return num;
        }


    }
}
