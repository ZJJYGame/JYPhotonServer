using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface ICharacterBattleData
    {
        int Hp { get; }
        int MaxHp { get; }
        int Mp { get; }
        int MaxMp { get; }
        int Soul { get; }
        int MaxSoul { get; }
        int BestBlood { get; }
        int BestBloodMax { get; }
        float AttackSpeed { get; }
        int PhysicalAtk { get; }
        int PhysicalDef { get; }
        int PowerAtk { get; }
        int PowerDef { get; }
        float PhysicalCritProb { get; }
        float MagicCritProb { get; }
        float ReduceCritProb { get; }
        int PhysicalCritDamage { get; }
        int MagicCritDamage { get; }
        int ReduceCritDamage { get; }
        int IgnoreDef { get; }
        int DamageAddition { get; }
        int DamageDeduction { get; }
        int Shield { get; }
        int DodgeProp { get;  }
        int PhysicDodgeProp { get; }
        int PowerDodgeProp { get; }
        int HealEffect { get; }
    }
}
