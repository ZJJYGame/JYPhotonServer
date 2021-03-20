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
