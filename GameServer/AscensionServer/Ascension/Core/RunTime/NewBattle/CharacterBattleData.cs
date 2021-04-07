using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionServer.Model;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    /// <summary>
    /// 用于进行战斗中角色的属性数据处理
    /// </summary>
    public class CharacterBattleData : IReference
    {
        int hp;
        public int Hp { get { return hp; } }
        int maxHp;
        public int MaxHp { get { return maxHp; } }
        int mp;
        public int Mp { get { return mp; } }
        int maxMp;
        public int MaxMp { get { return maxMp; } }
        int soul;
        public int Soul { get { return soul; } }
        int maxSoul;
        public int MaxSoul { get { return maxSoul; } }
        int bestBlood;
        public int BestBlood { get { return bestBlood; } }
        int bestBloodMax;
        public int BestBloodMax { get { return bestBloodMax; } }
        float attackSpeed;
        public float AttackSpeed { get { return attackSpeed; } }
        int physicalAtk;
        public int PhysicalAtk { get { return physicalAtk; } }
        int physicalDef;
        public int PhysicalDef { get { return physicalDef; } }
        int powerAtk;
        public int PowerAtk { get { return powerAtk; } }
        int powerDef;
        public int PowerDef { get { return powerDef; } }
        float physicalCritProb;
        public float PhysicalCritProb { get { return physicalCritProb; } }
        float magicCritProb;
        public float MagicCritProb { get { return magicCritProb; } }
        float reduceCritProb;
        public float ReduceCritProb { get { return reduceCritProb; } }
        int physicalCritDamage;
        public int PhysicalCritDamage { get { return physicalCritDamage; } }
        int magicCritDamage;
        public int MagicCritDamage { get { return magicCritDamage; } }
        int reduceCritDamage;
        public int ReduceCritDamage { get { return reduceCritDamage; } }
        int ignoreDef;
        public int IgnoreDef { get { return ignoreDef; } }
        int damageAddition;
        public int DamageAddition { get { return damageAddition; } }
        int damageDeduction;
        public int DamageDeduction { get { return damageDeduction; } }
        public int shield;
        public int Shield { get { return shield; } }
        /// <summary>
        /// 获取对应属性
        /// </summary>
        public float GetProperty(BattleSkillNumSourceType battleSkillNumSourceType,BattleDamageData battleDamageData)
        {
            switch (battleSkillNumSourceType)
            {
                case BattleSkillNumSourceType.MaxHealth:
                    return MaxHp;
                case BattleSkillNumSourceType.NowHealth:
                    return Hp;
                case BattleSkillNumSourceType.HasLostHealth:
                    return MaxHp - Hp;
                case BattleSkillNumSourceType.MaxZhenYuan:
                    return MaxMp;
                case BattleSkillNumSourceType.NowZhenYuan:
                    return Mp;
                case BattleSkillNumSourceType.MaxShenHun:
                    return MaxSoul;
                case BattleSkillNumSourceType.NowShenHun:
                    return Soul;
                case BattleSkillNumSourceType.PhysicAttack:
                    return PhysicalAtk;
                case BattleSkillNumSourceType.MagicAttack:
                    return PowerAtk;
                case BattleSkillNumSourceType.PhysicDefense:
                    return PhysicalDef;
                case BattleSkillNumSourceType.MagicDefense:
                    return PowerDef;
                case BattleSkillNumSourceType.AttackSpeed:
                    return AttackSpeed;
                case BattleSkillNumSourceType.TakeDamage:
                    return battleDamageData.damageNum;
                default:
                    return 0;
            }
        }
        public int GetProperty(BattleSkillDamageType battleSkillDamageType)
        {
            switch (battleSkillDamageType)
            {
                case BattleSkillDamageType.Physic:
                    return PhysicalDef;
                case BattleSkillDamageType.Magic:
                    return powerDef;
                case BattleSkillDamageType.ShenHun:
                    return 0;
                case BattleSkillDamageType.Reality:
                    return 0;
                default:
                    return 0;
            }
        }

        public int GetProperty(BattleSkillEventTriggerNumSourceType battleSkillEventTriggerNumSourceType)
        {
            switch (battleSkillEventTriggerNumSourceType)
            {
                case BattleSkillEventTriggerNumSourceType.Health:
                    return Hp;
                case BattleSkillEventTriggerNumSourceType.PhysicDefense:
                    return PhysicalDef;
                case BattleSkillEventTriggerNumSourceType.MagicDefense:
                    return PowerDef;
                case BattleSkillEventTriggerNumSourceType.ShenHun:
                    return Soul;
                case BattleSkillEventTriggerNumSourceType.Shield:
                    return Shield;
                default:
                    return 0;
            }
        }

        public int GetPropertyPercent(BattleSkillEventTriggerNumSourceType battleSkillEventTriggerNumSourceType)
        {
            switch (battleSkillEventTriggerNumSourceType)
            {
                case BattleSkillEventTriggerNumSourceType.Health:
                    Utility.Debug.LogError("获取血量百分比"+Hp+"/"+MaxHp+"/"+ Hp * 100 / MaxHp);
                    return Hp*100 / MaxHp;
                case BattleSkillEventTriggerNumSourceType.ShenHun:
                    return Mp*100 / MaxMp;
                case BattleSkillEventTriggerNumSourceType.PhysicDefense:
                case BattleSkillEventTriggerNumSourceType.MagicDefense:
                case BattleSkillEventTriggerNumSourceType.Shield:
                    return 100;
                default:
                    return 0;
            }
        }

        public void ChangeProperty(BattleSkillDamageTargetProperty baseDamageTargetProperty,int damageNum)
        {
            if (damageNum == 0)
                return;
            switch (baseDamageTargetProperty)
            {
                case BattleSkillDamageTargetProperty.Health:
                    hp += damageNum;
                    hp = hp < 0 ? 0 : hp;
                    hp = hp > MaxHp ? MaxHp : hp;
                    break;
                case BattleSkillDamageTargetProperty.ZhenYuan:
                    mp += damageNum;
                    mp = mp < 0 ? 0 : mp;
                    mp = mp > MaxMp ? MaxMp : mp;
                    break;
                case BattleSkillDamageTargetProperty.ShenHun:
                    soul += damageNum;
                    soul = soul < 0 ? 0 : soul;
                    soul = soul > MaxSoul ? MaxSoul : soul;
                    break;
            }
        }

        public void Init(RoleStatus roleStatus)
        {
            hp = roleStatus.RoleHP;
            maxHp = roleStatus.RoleMaxHP;
            mp = roleStatus.RoleMP;
            maxMp = roleStatus.RoleMaxMP;
            soul = roleStatus.RoleSoul;
            maxSoul = roleStatus.RoleMaxSoul;
            bestBlood = roleStatus.BestBlood;
            bestBloodMax = roleStatus.BestBloodMax;
            attackSpeed = roleStatus.AttackSpeed;
            physicalAtk = roleStatus.AttackPhysical;
            physicalDef = roleStatus.DefendPhysical;
            powerAtk = roleStatus.AttackPower;
            powerDef = roleStatus.DefendPower;
            physicalCritProb = roleStatus.PhysicalCritProb;
            magicCritProb = roleStatus.MagicCritProb;
            reduceCritProb = roleStatus.ReduceCritProb;
            physicalCritDamage = roleStatus.PhysicalCritDamage;
            magicCritDamage = roleStatus.MagicCritDamage;
            reduceCritDamage = roleStatus.ReduceCritDamage;
        }
        public void Init(PetStatus petStatus)
        {
            hp = petStatus.PetHP;
            maxHp = petStatus.PetMaxHP;
            mp = petStatus.PetMP;
            maxMp = petStatus.PetMaxMP;
            soul = petStatus.PetShenhun;
            maxSoul = petStatus.PetMaxShenhun;
            bestBlood = 0;
            bestBloodMax = 0;
            attackSpeed = petStatus.AttackSpeed;
            physicalAtk = petStatus.AttackPhysical;
            physicalDef = petStatus.DefendPhysical;
            powerAtk = petStatus.AttackPower;
            powerDef = petStatus.DefendPower;
            physicalCritProb = petStatus.PhysicalCritProb;
            magicCritProb = petStatus.MagicCritProb;
            reduceCritProb = petStatus.ReduceCritProb;
            physicalCritDamage = petStatus.PhysicalCritDamage;
            magicCritDamage = petStatus.MagicCritDamage;
            reduceCritDamage = petStatus.ReduceCritDamage;
        }
        public void Init(MonsterDatas monsterDatas)
        {
            hp = monsterDatas.Monster_HP;
            maxHp = monsterDatas.Monster_HP;
            mp = monsterDatas.Monster_MP;
            maxMp = monsterDatas.Monster_MP;
            soul = monsterDatas.Monster_Soul;
            maxSoul = monsterDatas.Monster_Soul;
            bestBlood = 0;
            bestBloodMax = 0;
            attackSpeed = monsterDatas.Attact_Speed;
            physicalAtk = monsterDatas.Attact_Physical;
            physicalDef = monsterDatas.Defend_Physical;
            powerAtk = monsterDatas.Attact_Power;
            powerDef = monsterDatas.Defend_Power;
            physicalCritProb = monsterDatas.PhysicalCritProb;
            magicCritProb = monsterDatas.MagicCritProb;
            reduceCritProb = monsterDatas.ReduceCritProb;
            physicalCritDamage = monsterDatas.PhysicalCritDamage;
            magicCritDamage = monsterDatas.MagicCritDamage;
            reduceCritDamage = monsterDatas.ReduceCritProb;
        }


        public void Clear()
        {
        }
    }
}
