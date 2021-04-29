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

            RoleStatusDTO roleStatus = new RoleStatusDTO();

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

        #region 加点方案属性修改
        /// <summary>
        /// 人物加点的加成
        /// </summary>
        /// <param name="pointDTO"></param>
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        public async Task<RoleStatus> RoleAblility(RoleStatusPointDTO pointDTO, RoleStatus roleStatus)
        {
            RoleStatusDTO roleStatusDTO = new RoleStatusDTO();
            roleStatusDTO.RoleID = roleStatus.RoleID;
            if (pointDTO.AbilityPointSln.TryGetValue(pointDTO.SlnNow, out var abilityDTO))
            {
                roleStatusDTO.RoleMaxHP += abilityDTO.Corporeity * 10;
                roleStatusDTO.RoleMaxMP += (abilityDTO.Power * 6 + abilityDTO.Corporeity);
                roleStatusDTO.RoleMaxSoul += abilityDTO.Soul * 4;
                roleStatusDTO.BestBloodMax += (short)(abilityDTO.Corporeity * 0.1);
                roleStatusDTO.AttackPhysical += abilityDTO.Strength * 2;
                roleStatusDTO.DefendPhysical += abilityDTO.Stamina * 2;
                roleStatusDTO.AttackPower += abilityDTO.Power * 2;
                roleStatusDTO.DefendPower += (int)(abilityDTO.Stamina * 0.8 + abilityDTO.Corporeity * 0.4 + abilityDTO.Power + abilityDTO.Strength * 1.2);
                roleStatusDTO.AttackSpeed += (int)(abilityDTO.Soul * 0.2 + abilityDTO.Stamina * 0.1 + abilityDTO.Corporeity * 0.1 + abilityDTO.Agility * 0.5 + abilityDTO.Strength * 0.1);
                roleStatusDTO.MoveSpeed += (int)(abilityDTO.Agility * 0.1);
            }
            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusAddPointPerfix, pointDTO.RoleID.ToString(), roleStatusDTO);
            var obj = RoleStatusAlgorithm(pointDTO.RoleID, null, null, null, roleStatusDTO, null);

            if (obj != null)
            {
                return StatusVerify(roleStatus, obj);
            }

            return new RoleStatus();
        }
        /// <summary>
        /// 切换加点
        /// </summary>
        /// <param name="pointDTO"></param>
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        public async Task<RoleStatus> RoleSwitchAblility(RoleStatusPointDTO pointDTO, RoleStatus roleStatus)
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
                roleStatusDTO.DefendPower += (int)(abilityDTO.Stamina * 0.8 + abilityDTO.Corporeity * 0.4 + abilityDTO.Power + abilityDTO.Strength * 1.2);
                roleStatusDTO.AttackSpeed += (int)(abilityDTO.Soul * 0.2 + abilityDTO.Stamina * 0.1 + abilityDTO.Corporeity * 0.1 + abilityDTO.Agility * 0.5 + abilityDTO.Strength * 0.1);
                roleStatusDTO.MoveSpeed += (int)(abilityDTO.Agility * 0.1);
            }
            var reduceStatus = new RoleStatus();
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusAddPointPerfix, pointDTO.RoleID.ToString()).Result)
            {
                var status = RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatusAddPointPerfix, pointDTO.RoleID.ToString()).Result;
                if (status != null)
                {
                    reduceStatus = status;
                }
            }

            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusAddPointPerfix, pointDTO.RoleID.ToString(), roleStatusDTO);



            var obj = RoleStatusAlgorithm(pointDTO.RoleID, null, null, null, roleStatusDTO, null);
            Utility.Debug.LogInfo("加成的数据的 数据1" + Utility.Json.ToJson(obj));
            if (obj != null)
            {
                return StatusVerify(roleStatus, obj, reduceStatus, roleStatusDTO);
            }

            return new RoleStatus();
        }
        #endregion

        #region 角色装备属性计算
        public RoleStatusAdditionDTO Addition<T>(T data, RoleStatusAdditionDTO roleStatus) where T : IPassiveSkills
        {
            for (int j = 0; j < data.Attribute.Count; j++)
            {
                var type = data.Attribute[j];
                var Fixed = data.Fixed;
                var Percentage = data.Percentage;
                if (roleStatus.Percentage.ContainsKey(type))
                    roleStatus.Percentage[type] += Percentage[j];
                else
                    roleStatus.Percentage.TryAdd(type, Percentage[j]);
                switch ((RoleStatusType)type)
                {
                    case RoleStatusType.RoleHP:
                        roleStatus.RoleMaxHP += Fixed[j];
                        break;
                    case RoleStatusType.RoleMP:
                        roleStatus.RoleMaxMP += Fixed[j];
                        break;
                    case RoleStatusType.RoleSoul:
                        roleStatus.RoleMaxSoul += Fixed[j];
                        break;
                    case RoleStatusType.AttackPhysical:
                        roleStatus.AttackPhysical += Fixed[j];
                        break;
                    case RoleStatusType.DefendPhysical:
                        roleStatus.DefendPhysical += Fixed[j];
                        break;
                    case RoleStatusType.AttackPower:
                        roleStatus.AttackPower += Fixed[j];
                        break;
                    case RoleStatusType.DefendPower:
                        roleStatus.DefendPower += Fixed[j];
                        break;
                    case RoleStatusType.AttackSpeed:
                        roleStatus.AttackSpeed += Fixed[j];
                        break;
                    case RoleStatusType.PhysicalCritProb:
                        roleStatus.PhysicalCritProb += Fixed[j];
                        break;
                    case RoleStatusType.PhysicalCritDamage:
                        roleStatus.PhysicalCritDamage += Fixed[j];
                        break;
                    case RoleStatusType.MagicCritProb:
                        roleStatus.MagicCritProb += Fixed[j];
                        break;
                    case RoleStatusType.MagicCritDamage:
                        roleStatus.MagicCritDamage += Fixed[j];
                        break;
                    case RoleStatusType.ReduceCritProb:
                        roleStatus.ReduceCritProb += Fixed[j];
                        break;
                    case RoleStatusType.ReduceCritDamage:
                        roleStatus.ReduceCritDamage += Fixed[j];
                        break;
                    case RoleStatusType.MoveSpeed:
                        roleStatus.MoveSpeed += Fixed[j];
                        break;
                    case RoleStatusType.GongfaLearnSpeed:
                        roleStatus.GongfaLearnSpeed += Fixed[j];
                        break;
                    case RoleStatusType.MishuLearnSpeed:
                        roleStatus.MishuLearnSpeed += Fixed[j];
                        break;
                    default:
                        break;
                }
            }

            return roleStatus;
        }

        public WeaponDTO AdditionWeapon(WeaponDTO addDTO, WeaponDTO targetDTO, PassiveSkillsWeapon SkillData)
        {

            for (int i = 0; i < SkillData.Attribute.Count; i++)
            {
                switch ((RoleStatusType)SkillData.Attribute[i])
                {
                    case RoleStatusType.RoleHP:
                        targetDTO.WeaponAttribute[0] += addDTO.WeaponAttribute[0] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        break;
                    case RoleStatusType.AttackPhysical:
                        targetDTO.WeaponAttribute[1] += addDTO.WeaponAttribute[1] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        Utility.Debug.LogError("YZQ装备法宝后的属性加成1" + addDTO.WeaponAttribute[1] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i]);
                        break;
                    case RoleStatusType.DefendPhysical:
                        targetDTO.WeaponAttribute[2] += addDTO.WeaponAttribute[2] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        break;
                    case RoleStatusType.AttackPower:
                        targetDTO.WeaponAttribute[3] += addDTO.WeaponAttribute[3] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        break;
                    case RoleStatusType.DefendPower:
                        targetDTO.WeaponAttribute[4] += addDTO.WeaponAttribute[4] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        break;
                    case RoleStatusType.AttackSpeed:
                        targetDTO.WeaponAttribute[5] += addDTO.WeaponAttribute[5] * SkillData.Percentage[i] / 100 + SkillData.Fixed[i];
                        break;
                }
            }

            for (int i = 0; i < targetDTO.WeaponAttribute.Count; i++)
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

        /// <summary>
        /// 人物裝備裝備的屬性計算
        /// </summary>
        /// <param name="statusObj"></param>
        /// <param name="roleWeapon"></param>
        /// <param name="roleEquipment"></param>
        /// <returns></returns>
        public async Task<RoleStatus> RoleEquip(RoleStatus statusObj,RoleWeaponDTO roleWeapon, RoleEquipmentDTO roleEquipment)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PassiveSkillsWeapon>>(out var weaponSkillDict);
            RoleStatusAdditionDTO roleStatus = new RoleStatusAdditionDTO();
            WeaponDTO weapon = new WeaponDTO();
            weapon.WeaponAttribute=new List<int> { 0, 0, 0, 0, 0, 0 };

            var Weaponlist = new List<WeaponDTO>();

            foreach (var item in roleEquipment.Weapon)
            {
                if (roleWeapon.WeaponStatusDict.TryGetValue(item.Value, out var weaponDTO))
                {
                    Weaponlist.Add(weaponDTO);
                }
            }
            Utility.Debug.LogError("YZQ装备法宝后的属性加成y" + Utility.Json.ToJson(Weaponlist));
            for (int i = 0; i < Weaponlist.Count; i++)
            {
                for (int j = 0; j < Weaponlist[i].WeaponSkill.Count; j++)
                {
                    if (weaponSkillDict.TryGetValue(Weaponlist[i].WeaponSkill[j],out var skill))
                    {
                        switch (skill.AddTarget)
                        {
                            case 1:
                                roleStatus = Addition(skill, roleStatus);
                                break;
                            case 0:
                                Utility.Debug.LogError("YZQ装备法宝后的属性加成z" + Utility.Json.ToJson(skill));
                                weapon = AdditionWeapon(Weaponlist[i], weapon, skill);
                                break;
                        }
                    }
                  
                }
            }


            roleStatus = AdditionWeaponStatus(weapon, roleStatus);

            var status= RoleStatusAlgorithm(statusObj.RoleID, null,null,null,null, roleStatus);

            Utility.Debug.LogError("YZQ装备法宝后的属性加成2"+Utility.Json.ToJson(status));

          await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusEquipPerfix, statusObj.RoleID.ToString(), roleStatus);

            return StatusVerify(statusObj, status,new RoleStatus(),new RoleStatusDTO()); 
        }
        #endregion

        #region 角色飞行法器
        public async Task<RoleStatus> RoleFlyMagicTool(FlyMagicToolDTO flyMagic, RoleStatus roleStatus)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, FlyMagicToolData>>(out var flytool);
            RoleStatusDTO Status = new RoleStatusDTO();
            foreach (var item in flyMagic.FlyToolLayoutDict)
            {
                if (flytool.TryGetValue(item.Value, out var flyMagicToolData))
                {
                    Status.AttackPhysical += flyMagicToolData.AddPhysicAttack;
                    Status.AttackPower += flyMagicToolData.AddMagicAttack;
                    Status.MoveSpeed += flyMagicToolData.AddMoveSpeed;
                    Status.RoleMaxHP += flyMagicToolData.AddRoleHp;
                }
            }
            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusFlyPerfix,roleStatus.RoleID.ToString(), Status);

            var rolestatusObj = RoleStatusAlgorithm(Status.RoleID, Status, null,null, null, null);
            if (rolestatusObj!=null)
            {
                return StatusVerify(roleStatus, rolestatusObj);
            }
            return new RoleStatus();
        }

        /// <summary>
        /// 切换飞行法器
        /// </summary>
        /// <param name="flyMagic"></param>
        /// <param name="roleStatus"></param>
        /// <returns></returns>
        public async Task<RoleStatus> RoleSwitchFlyMagicTool(FlyMagicToolDTO flyMagic, RoleStatus roleStatus)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, FlyMagicToolData>>(out var flytool);
            RoleStatusDTO Status = new RoleStatusDTO();
            foreach (var item in flyMagic.FlyToolLayoutDict)
            {
                if (flytool.TryGetValue(item.Value, out var flyMagicToolData))
                {
                    Status.AttackPhysical += flyMagicToolData.AddPhysicAttack;
                    Status.AttackPower += flyMagicToolData.AddMagicAttack;
                    Status.MoveSpeed += flyMagicToolData.AddMoveSpeed;
                    Status.RoleMaxHP += flyMagicToolData.AddRoleHp;
                }
            }
            var reduceStatus = new RoleStatus();
            if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusFlyPerfix, roleStatus.RoleID.ToString()).Result)
            {
                var status = RedisHelper.Hash.HashGetAsync<RoleStatus>(RedisKeyDefine._RoleStatusFlyPerfix, roleStatus.RoleID.ToString()).Result;
                if (status != null)
                {
                    reduceStatus = status;
                }
            }


            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RoleStatusFlyPerfix, roleStatus.RoleID.ToString(), Status);

            var rolestatusObj = RoleStatusAlgorithm(Status.RoleID, Status, null, null, null, null);
            return StatusVerify(roleStatus, rolestatusObj, reduceStatus, Status);
        }
        #endregion

        /// <summary>
        /// 计算技能加成
        /// </summary>
        /// <summary>
        /// 计算技能加成
        /// </summary>
        public RoleStatusAdditionDTO SkillAlgorithm(int roleid, RoleStatus status, RoleGongFaDTO rolegongfa, RoleMiShuDTO roleMiShu)
        {
            List<int> gongfaList = new List<int>();
            List<int> mishuList = new List<int>();
            List<int> ids = new List<int>();

            RoleStatusDTO roleStatusGF = new RoleStatusDTO();
            RoleStatusDTO roleStatusMS = new RoleStatusDTO();
            GameEntry.DataManager.TryGetValue<Dictionary<int, GongFa>>(out var gongfa);
            GameEntry.DataManager.TryGetValue<Dictionary<int, MiShuData>>(out var mishu);


            gongfaList = rolegongfa.GongFaIDDict.Keys.ToList();
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
                    roleStatusGF.AttackPhysical += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.AttackPower += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.AttackSpeed += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.BestBlood += (short)gongfa[gongfaList[j]].Best_Blood;
                    roleStatusGF.DefendPhysical += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.DefendPower += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.MagicCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.MagicCritProb += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.MaxVitality += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.MoveSpeed += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.PhysicalCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.PhysicalCritProb += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.ReduceCritDamage += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.ReduceCritProb += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.RoleHP += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.RoleMP += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.RolePopularity += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.RoleSoul += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.ValueHide += gongfa[gongfaList[j]].Attact_Physical;
                    roleStatusGF.Vitality += gongfa[gongfaList[j]].Attact_Physical;

                    roleStatusGF.RoleMaxHP = roleStatusMS.RoleHP;
                    roleStatusGF.RoleMaxMP = roleStatusMS.RoleMP;
                    roleStatusGF.RoleMaxPopularity = roleStatusMS.RolePopularity;
                    roleStatusGF.RoleMaxSoul = roleStatusMS.RoleSoul;
                    roleStatusGF.BestBloodMax = roleStatusMS.BestBlood;
                }
            }


















            var roleWeaponExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;

            var roleEquipmentExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleEquipmentPerfix, roleid.ToString()).Result;

            if (roleWeaponExist && roleEquipmentExist)
            {
                var roleWeaponDTO = RedisHelper.Hash.HashGetAsync<RoleWeaponDTO>(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;
                var roleEquipmentDTO = RedisHelper.Hash.HashGetAsync<RoleEquipmentDTO>(RedisKeyDefine._RoleWeaponPostfix, roleid.ToString()).Result;

                if (roleWeaponDTO != null && roleEquipmentDTO != null)
                {
                    GameEntry.DataManager.TryGetValue<Dictionary<int, PassiveSkillsRole>>(out var roleskillDict);
                    RoleStatusAdditionDTO roleStatus = new RoleStatusAdditionDTO();
                    var WeaponDict = new Dictionary<int, WeaponDTO>();
                    for (int i = 0; i < ids.Count; i++)
                    {
                        foreach (var item in roleEquipmentDTO.Weapon.Values)
                            WeaponDict.TryAdd(roleWeaponDTO.WeaponStatusDict[item].WeaponType, roleWeaponDTO.WeaponStatusDict[item]);

                        if (WeaponDict.ContainsKey(roleskillDict[ids[i]].WeaponType))
                         roleStatus = Addition(roleskillDict[ids[i]], roleStatus); 
                    }
                    return roleStatus;
                }
                else
                    return null;
            }
            else
                return null;
        }
        /// <summary>
        /// 获取Redis各部分加成
        /// </summary>
        /// <param name="statusFly">飞行法器加成</param>
        /// <param name="statusGF">功法加成</param>
        /// <param name="statusMS">秘术加成</param>
        /// <param name="statusPoint">加点加成</param>
        /// <param name="statusEquip">装备加成</param>
        public  RoleStatusDTO RoleStatusAlgorithm(int roleid, RoleStatusDTO statusFly = null, RoleStatusAdditionDTO statusGF = null, RoleStatusAdditionDTO statusMS = null, RoleStatusDTO statusPoint = null, RoleStatusAdditionDTO statusEquip = null,RoleAllianceSkill roleAllianceSkill=null)
        {
            SkillsData hp=new SkillsData();
            SkillsData soul = new SkillsData();
            SkillsData mp = new SkillsData();
            SkillsData attackspeed = new SkillsData();

            GameEntry.DataManager.TryGetValue<Dictionary<int, RoleLevelData>>(out var roleData);
            GameEntry.DataManager.TryGetValue<Dictionary<string, AllianceSkillsData>>(out var allianceSkiil);
            var roleexist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePostfix,roleid.ToString()).Result;
            if (roleexist)
            {
                var role = RedisHelper.Hash.HashGetAsync<RoleDTO>(RedisKeyDefine._RolePostfix, roleid.ToString()).Result;
                if (role!=null)
                {
                    if (statusFly == null)
                    {
                        Utility.Debug.LogError("123");
                        var flyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusFlyPerfix, roleid.ToString()).Result;
                        if (flyExist)
                        {
                            Utility.Debug.LogError("234");
                            statusFly = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatusFlyPerfix, roleid.ToString()).Result;
                            if (statusFly == null)
                            {
                                statusFly = new RoleStatusDTO();

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
                            if (statusGF == null)
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
                            if (statusMS == null)
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
                            if (statusEquip == null)
                            {
                                statusEquip = new RoleStatusAdditionDTO();
                            }
                        }
                        else statusEquip = new RoleStatusAdditionDTO();
                    }
                    if (statusPoint == null)
                    {
                        var statusPointExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                        if (statusPointExist)
                        {
                            statusPoint = RedisHelper.Hash.HashGetAsync<RoleStatusDTO>(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                            if (statusPoint == null)
                            {
                                statusPoint = new RoleStatusDTO();
                            }
                        }
                        else statusPoint = new RoleStatusDTO();
                    }
                    if (roleAllianceSkill == null)
                    {
                        var roleAllianceSkillExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                        if (roleAllianceSkillExist)
                        {
                            roleAllianceSkill = RedisHelper.Hash.HashGetAsync<RoleAllianceSkill>(RedisKeyDefine._RoleStatusAddPointPerfix, roleid.ToString()).Result;
                            if (roleAllianceSkill == null)
                            {
                                roleAllianceSkill = new RoleAllianceSkill();
                            }
                        }
                        else roleAllianceSkill = new RoleAllianceSkill();
                    }

                    if (allianceSkiil.TryGetValue("StrongBody", out var StrongBody))
                    {
                        hp = StrongBody.AllianceSkillData.Find((x)=>x.SkillLevel== roleAllianceSkill.SkillStrong);
                    }

                    if (allianceSkiil.TryGetValue("Insight", out var Insight))
                    {
                         soul = Insight.AllianceSkillData.Find((x) => x.SkillLevel == roleAllianceSkill.SkillStrong);
                    }

                    if (allianceSkiil.TryGetValue("Rapid", out var Rapid))
                    {
                         attackspeed = Rapid.AllianceSkillData.Find((x) => x.SkillLevel == roleAllianceSkill.SkillStrong);
                    }

                    if (allianceSkiil.TryGetValue("Meditation", out var Meditation))
                    {
                         mp = Meditation.AllianceSkillData.Find((x) => x.SkillLevel == roleAllianceSkill.SkillStrong);
                    }

                    RoleStatusType statusType = new RoleStatusType();
                    RoleStatusDTO roleStatus = new RoleStatusDTO();
                    if (roleData.TryGetValue(role.RoleLevel, out var roleObj))
                    {
                        #region 
                        roleStatus.AttackPhysical += statusFly.AttackPhysical + statusGF.AttackPhysical + statusEquip.AttackPhysical + statusMS.AttackPhysical + statusPoint.AttackPhysical+ roleObj.AttackPhysical;
                        roleStatus.AttackPower += statusFly.AttackPower + roleObj.AttackPower + statusGF.AttackPower + statusEquip.AttackPower + statusMS.AttackPower + statusPoint.AttackPower;
                        roleStatus.AttackSpeed += statusFly.AttackSpeed + roleObj.AttackSpeed + statusGF.AttackSpeed + statusEquip.AttackSpeed + statusMS.AttackSpeed + statusPoint.AttackSpeed+ attackspeed.AddCoefficient* roleAllianceSkill.SkillRapid;
                        roleStatus.BestBloodMax += (short)(statusFly.BestBlood + roleObj.BestBlood + statusGF.BestBlood + statusEquip.BestBlood + statusMS.BestBlood + statusPoint.BestBlood);
                        roleStatus.DefendPhysical += statusFly.DefendPhysical + roleObj.DefendPhysical + statusGF.DefendPhysical + statusEquip.DefendPhysical + statusMS.DefendPhysical + statusPoint.DefendPhysical;
                        roleStatus.DefendPower += statusFly.DefendPower + roleObj.DefendPower + statusGF.DefendPower + statusEquip.DefendPower + statusMS.DefendPower + statusPoint.DefendPower;
                        roleStatus.GongfaLearnSpeed += statusFly.GongfaLearnSpeed + roleObj.GongfaLearnSpeed + statusGF.GongfaLearnSpeed + statusEquip.GongfaLearnSpeed + statusMS.GongfaLearnSpeed + statusPoint.GongfaLearnSpeed;
                        roleStatus.MagicCritDamage += statusFly.MagicCritDamage + statusGF.MagicCritDamage + roleObj.MagicCritDamage + statusEquip.MagicCritDamage + statusMS.MagicCritDamage + statusPoint.MagicCritDamage;
                        roleStatus.MagicCritProb += statusFly.MagicCritProb + statusGF.MagicCritProb + statusEquip.MagicCritProb + roleObj.MagicCritProb + statusMS.MagicCritProb + statusPoint.MagicCritProb;
                        roleStatus.MishuLearnSpeed += statusFly.MishuLearnSpeed + roleObj.MishuLearnSpeed + statusGF.MishuLearnSpeed + statusEquip.MishuLearnSpeed + statusMS.MishuLearnSpeed + statusPoint.MishuLearnSpeed;
                        roleStatus.MoveSpeed += statusFly.MoveSpeed + roleObj.MoveSpeed + statusGF.MoveSpeed + statusEquip.MoveSpeed + statusMS.MoveSpeed + statusPoint.MoveSpeed;
                        roleStatus.PhysicalCritDamage += statusFly.PhysicalCritDamage + roleObj.PhysicalCritDamage + statusGF.PhysicalCritDamage + statusEquip.PhysicalCritDamage + statusMS.PhysicalCritDamage + statusPoint.PhysicalCritDamage;
                        roleStatus.PhysicalCritProb += statusFly.PhysicalCritProb + roleObj.PhysicalCritProb + statusGF.PhysicalCritProb + statusEquip.PhysicalCritProb + statusMS.PhysicalCritProb + statusPoint.PhysicalCritProb;
                        roleStatus.ReduceCritDamage += statusFly.ReduceCritDamage + roleObj.ReduceCritDamage + statusGF.ReduceCritDamage + statusEquip.ReduceCritDamage + statusMS.ReduceCritDamage + statusPoint.ReduceCritDamage;
                        roleStatus.ReduceCritProb += statusFly.ReduceCritProb + statusGF.ReduceCritProb + roleObj.ReduceCritProb + statusEquip.ReduceCritProb + statusMS.ReduceCritProb + statusPoint.ReduceCritProb;
                        roleStatus.RoleMaxHP += statusFly.RoleMaxHP+ roleObj.RoleHP + statusGF.RoleMaxHP + statusEquip.RoleMaxHP + statusMS.RoleMaxHP + statusPoint.RoleMaxHP+ hp.AddCoefficient* roleAllianceSkill.SkillStrong;
                        roleStatus.RolePopularity += statusFly.RoleMaxPopularity+ roleObj.RolePopularity + statusGF.RoleMaxPopularity + statusEquip.RoleMaxPopularity + statusMS.RoleMaxPopularity + statusPoint.RoleMaxPopularity;
                        roleStatus.RoleMaxSoul += statusFly.RoleMaxSoul + statusGF.RoleMaxSoul + roleObj .RoleSoul+ statusEquip.RoleMaxSoul + statusMS.RoleMaxSoul + statusPoint.RoleMaxSoul+ soul.AddCoefficient * roleAllianceSkill.SkillInsight;
                        roleStatus.ValueHide += statusFly.ValueHide + roleObj .ValueHide+ statusGF.ValueHide + statusEquip.ValueHide + statusMS.ValueHide + statusPoint.ValueHide;
                        roleStatus.MaxVitality += statusFly.MaxVitality + statusGF.MaxVitality + statusEquip.MaxVitality + statusMS.MaxVitality + statusPoint.MaxVitality + roleObj.Vitality;
                        roleStatus.RoleMaxMP += statusFly.RoleMaxMP + statusGF.RoleMaxMP + statusEquip.RoleMaxMP + statusMS.RoleMaxMP + statusPoint.RoleMaxMP + roleObj.RoleMP+ mp.AddCoefficient* roleAllianceSkill.SkillMeditation;
                        #endregion
                        var Percentage = 0;

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
                        roleStatus.RoleID = roleid;
                        return roleStatus;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;


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

        /// <summary>
        /// 切换加点方案武器装备飞行法器等
        /// </summary>
        /// <param name="roleStatus"></param>
        /// <param name="obj"></param>
        /// <param name="reducesStatus"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public RoleStatus StatusVerify(RoleStatus roleStatus,RoleStatusDTO obj,RoleStatus reducesStatus,RoleStatusDTO status)
        {
            Utility.Debug.LogInfo("数据库获取的 数据" + Utility.Json.ToJson(roleStatus));
            Utility.Debug.LogInfo("加成的数据的 数据" + Utility.Json.ToJson(obj));
            Utility.Debug.LogInfo("替换减少的 数据" + Utility.Json.ToJson(reducesStatus));
            if (status.RoleMaxHP - reducesStatus.RoleMaxHP > 0)
            {
                roleStatus.RoleHP += obj.RoleMaxHP - reducesStatus.RoleMaxHP - roleStatus.RoleMaxHP;
                roleStatus.RoleMaxHP = obj.RoleMaxHP - reducesStatus.RoleMaxHP;
            }
            else
            {
                roleStatus.RoleHP += obj.RoleMaxHP - roleStatus.RoleMaxHP;
                roleStatus.RoleMaxHP = obj.RoleMaxHP;
            }

            if (status.RoleMaxMP - reducesStatus.RoleMaxMP > 0)
            {

                roleStatus.RoleMP += (obj.RoleMaxMP - reducesStatus.RoleMaxMP - roleStatus.RoleMaxMP);
                roleStatus.RoleMaxMP = obj.RoleMaxMP - reducesStatus.RoleMaxMP;
            }
            else
            {
                roleStatus.RoleMP += (obj.RoleMaxMP  - roleStatus.RoleMaxMP);
                roleStatus.RoleMaxMP = obj.RoleMaxMP;
            }

            if (status.RoleMaxSoul - reducesStatus.RoleMaxSoul > 0)
            {
                roleStatus.RoleSoul += (obj.RoleMaxSoul - reducesStatus.RoleMaxSoul - roleStatus.RoleMaxSoul);
                roleStatus.RoleMaxSoul = obj.RoleMaxSoul - reducesStatus.RoleMaxSoul;
            }
            else
            {
                roleStatus.RoleSoul += (obj.RoleMaxSoul - roleStatus.RoleMaxSoul);
                roleStatus.RoleMaxSoul = obj.RoleMaxSoul ;
            }

            if (status.BestBloodMax - reducesStatus.BestBloodMax > 0)
            {
                roleStatus.BestBlood += (obj.BestBloodMax - reducesStatus.BestBloodMax - roleStatus.BestBloodMax);
                roleStatus.BestBloodMax = (obj.BestBloodMax - reducesStatus.BestBloodMax);
            }
            else
            {
                roleStatus.BestBlood += (obj.BestBloodMax  - roleStatus.BestBloodMax);
                roleStatus.BestBloodMax = (obj.BestBloodMax );
            }
            if (status.MaxVitality - reducesStatus.MaxVitality>0)
            {
                roleStatus.Vitality += (short)(obj.MaxVitality - reducesStatus.MaxVitality - roleStatus.MaxVitality);
                roleStatus.MaxVitality = obj.MaxVitality - reducesStatus.MaxVitality;
            }
            else
            {
                roleStatus.Vitality += (short)(obj.MaxVitality  - roleStatus.MaxVitality);
                roleStatus.MaxVitality = obj.MaxVitality ;
            }
            if (status.AttackPhysical - reducesStatus.AttackPhysical > 0)
            {
                roleStatus.AttackPhysical = obj.AttackPhysical - reducesStatus.AttackPhysical;
            }
            else
                roleStatus.AttackPhysical = obj.AttackPhysical;
            if (obj.AttackPower - reducesStatus.AttackPower > 0)
            {
                roleStatus.AttackPower = obj.AttackPower - reducesStatus.AttackPower;
            }
            else
                roleStatus.AttackPower = obj.AttackPower;

            if (status.AttackSpeed - reducesStatus.AttackSpeed>0)
            {
                roleStatus.AttackSpeed = obj.AttackSpeed - reducesStatus.AttackSpeed;
            }else
                roleStatus.AttackSpeed = obj.AttackSpeed ;
            if (status.AttackSpeed - reducesStatus.AttackSpeed > 0)
            {
                roleStatus.AttackSpeed = obj.AttackSpeed - reducesStatus.AttackSpeed;
            }else
                roleStatus.AttackSpeed = obj.AttackSpeed;

            if (status.DefendPower - reducesStatus.DefendPower > 0)
            {
                roleStatus.DefendPower = obj.DefendPower - reducesStatus.DefendPower;
            }
            else
                roleStatus.DefendPower = obj.DefendPower;


            if (status.DefendPhysical - reducesStatus.DefendPhysical>0)
            {
                roleStatus.DefendPhysical = obj.DefendPhysical - reducesStatus.DefendPhysical;
            }
            else
                roleStatus.DefendPhysical = obj.DefendPhysical;
            if (status.GongfaLearnSpeed - reducesStatus.GongfaLearnSpeed>0)
            {
                roleStatus.GongfaLearnSpeed = obj.GongfaLearnSpeed - reducesStatus.GongfaLearnSpeed;
            }else
                roleStatus.GongfaLearnSpeed = obj.GongfaLearnSpeed ;
            if (status.MagicCritDamage - reducesStatus.MagicCritDamage>0)
            {
                roleStatus.MagicCritDamage = obj.MagicCritDamage - reducesStatus.MagicCritDamage;
            }else
                roleStatus.MagicCritDamage = obj.MagicCritDamage ;
            if (status.MagicCritDamage - reducesStatus.MagicCritDamage>0)
            {
                roleStatus.MagicCritDamage = obj.MagicCritDamage - reducesStatus.MagicCritDamage;
            }else
                roleStatus.MagicCritDamage = obj.MagicCritDamage ;
            if (status.MagicCritProb - reducesStatus.MagicCritProb>0)
            {
                roleStatus.MagicCritProb = obj.MagicCritProb - reducesStatus.MagicCritProb;
            }else
                roleStatus.MagicCritProb = obj.MagicCritProb ;
            if (status.MishuLearnSpeed - reducesStatus.MishuLearnSpeed>0)
            {
                roleStatus.MishuLearnSpeed = obj.MishuLearnSpeed - reducesStatus.MishuLearnSpeed;
            }else
                roleStatus.MishuLearnSpeed = obj.MishuLearnSpeed ;
            if (status.MoveSpeed - reducesStatus.MoveSpeed>0)
            {
                roleStatus.MoveSpeed = obj.MoveSpeed - reducesStatus.MoveSpeed;
            }else
                roleStatus.MoveSpeed = obj.MoveSpeed ;
            if (status.PhysicalCritDamage - reducesStatus.PhysicalCritDamage>0)
            {
                roleStatus.PhysicalCritDamage = obj.PhysicalCritDamage - reducesStatus.PhysicalCritDamage;
            }else
                roleStatus.PhysicalCritDamage = obj.PhysicalCritDamage ;
            if (status.PhysicalCritProb - reducesStatus.PhysicalCritProb>0)
            {
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb - reducesStatus.PhysicalCritProb;
            }else
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb;

            if (status.PhysicalCritProb - reducesStatus.PhysicalCritProb>0)
            {
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb - reducesStatus.PhysicalCritProb;
            }else
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb ;

            if (status.PhysicalCritProb - reducesStatus.PhysicalCritProb>0)
            {
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb - reducesStatus.PhysicalCritProb;
            }else
                roleStatus.PhysicalCritProb = obj.PhysicalCritProb ;

            if (status.ReduceCritDamage - reducesStatus.ReduceCritDamage>0)
            {
                roleStatus.ReduceCritDamage = obj.ReduceCritDamage - reducesStatus.ReduceCritDamage;
            }else
                roleStatus.ReduceCritDamage = obj.ReduceCritDamage;

            if (status.ReduceCritDamage - reducesStatus.ReduceCritDamage>0)
            {
                roleStatus.ReduceCritDamage = obj.ReduceCritDamage - reducesStatus.ReduceCritDamage;
            }else
                roleStatus.ReduceCritDamage = obj.ReduceCritDamage ;

            if (status.ReduceCritProb - reducesStatus.ReduceCritProb>0)
            {
                roleStatus.ReduceCritProb = obj.ReduceCritProb - reducesStatus.ReduceCritProb;
            }else
                roleStatus.ReduceCritProb = obj.ReduceCritProb;

            if (status.RolePopularity - reducesStatus.RolePopularity>0)
            {
                roleStatus.RolePopularity = obj.RolePopularity - reducesStatus.RolePopularity;
            }else
                roleStatus.RolePopularity = obj.RolePopularity ;

            if (status.ValueHide - reducesStatus.ValueHide>0)
            {
                roleStatus.ValueHide = obj.ValueHide - reducesStatus.ValueHide;
            }else
                roleStatus.ValueHide = obj.ValueHide ;

            return roleStatus;
        }
        /// <summary>
        /// 正常加点装备装备飞行法器
        /// </summary>
        /// <param name="roleStatus"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public RoleStatus StatusVerify(RoleStatus roleStatus, RoleStatusDTO obj)
        {
 
            roleStatus.RoleHP += (obj.RoleMaxHP - roleStatus.RoleMaxHP);
            roleStatus.RoleMaxHP = obj.RoleMaxHP;

            roleStatus.RoleMP += (obj.RoleMaxMP - roleStatus.RoleMaxMP);
            roleStatus.RoleMaxMP = obj.RoleMaxMP;


            roleStatus.RoleSoul += (obj.RoleMaxSoul - roleStatus.RoleMaxSoul);
            roleStatus.RoleMaxSoul = obj.RoleMaxSoul;


            roleStatus.BestBlood += (short)(obj.BestBloodMax - roleStatus.BestBloodMax);
            roleStatus.BestBloodMax = obj.BestBloodMax;


            roleStatus.Vitality += obj.Vitality - roleStatus.MaxVitality;
            roleStatus.MaxVitality = obj.MaxVitality;


            roleStatus.RolePopularity += obj.RoleMaxPopularity - roleStatus.RoleMaxPopularity;
            roleStatus.RoleMaxPopularity = obj.RoleMaxPopularity;


            roleStatus.AttackPhysical = obj.AttackPhysical;
            roleStatus.AttackPower = obj.AttackPower;
            roleStatus.AttackSpeed = obj.AttackSpeed;
            roleStatus.DefendPower = obj.DefendPower;
            roleStatus.DefendPhysical = obj.DefendPhysical;
            roleStatus.GongfaLearnSpeed = obj.GongfaLearnSpeed;
            roleStatus.MagicCritDamage = obj.MagicCritDamage;
            roleStatus.MagicCritProb = obj.MagicCritProb;
            roleStatus.MishuLearnSpeed = obj.MishuLearnSpeed;
            roleStatus.MoveSpeed = obj.MoveSpeed;
            roleStatus.PhysicalCritDamage = obj.PhysicalCritDamage;
            roleStatus.PhysicalCritProb = obj.PhysicalCritProb;
            roleStatus.ReduceCritDamage = obj.ReduceCritDamage;
            roleStatus.ReduceCritProb = obj.ReduceCritProb;
            roleStatus.ValueHide = obj.ValueHide;

           ;

            return roleStatus;
        }
    }
}
