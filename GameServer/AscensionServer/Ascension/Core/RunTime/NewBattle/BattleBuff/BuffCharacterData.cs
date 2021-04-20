using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class BuffCharacterData : ICharacterBattleData
    {
        //角色的属性
        ICharacterBattleData roleBattleData;


        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Mp { get; private set; }
        public int MaxMp { get; private set; }
        public int Soul { get; private set; }
        public int MaxSoul { get; private set; }
        public int BestBlood { get; private set; }
        public int BestBloodMax { get; private set; }
        public float AttackSpeed { get; private set; }
        public int PhysicalAtk { get; private set; }
        public int PhysicalDef { get; private set; }
        public int PowerAtk { get; private set; }
        public int PowerDef { get; private set; }
        public float PhysicalCritProb { get; private set; }
        public float MagicCritProb { get; private set; }
        public float ReduceCritProb { get; private set; }
        public int PhysicalCritDamage { get; private set; }
        public int MagicCritDamage { get; private set; }
        public int ReduceCritDamage { get; private set; }
        public int IgnoreDef { get; private set; }
        public int DamageAddition { get; private set; }
        public int DamageDeduction { get; private set; }
        public int Shield { get; private set; }
        public int DodgeProp { get; private set; }
        public int PhysicDodgeProp { get; private set; }
        public int PowerDodgeProp { get; private set; }
        public int HealEffect { get; private set; }

        public void ChangeProperty(float ChangeValue, BattleBuffEventType_RolePropertyChange battleBuffEventType_RolePropertyChange)
        {
            switch (battleBuffEventType_RolePropertyChange)
            {
                case BattleBuffEventType_RolePropertyChange.Health:
                    Hp +=(int) ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.ZhenYuan:
                    Mp += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.ShenHun:
                    Soul += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.JingXue:
                    BestBlood += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicAttack:
                    PhysicalAtk += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicDefend:
                    PhysicalDef += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicAttack:
                    PowerAtk += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicDefend:
                    PowerDef += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.AttackSpeed:
                    AttackSpeed += ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.HitRate:
                    //todo 命中率加成待处理
                    break;
                case BattleBuffEventType_RolePropertyChange.BasicDodgeRate:
                    DodgeProp += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicDodgeRate:
                    PhysicDodgeProp += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicDodgeRate:
                    PowerDodgeProp += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicCritRate:
                    PhysicalCritProb += ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicCritRate:
                    MagicCritProb += ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.ReduceCritRate:
                    ReduceCritProb += ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.PhysicCritDamage:
                    PhysicalCritDamage += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.MagicCritDamage:
                    MagicCritDamage += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.ReduceCritDamage:
                    ReduceCritDamage += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.TakeDamage:
                    DamageAddition += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.ReceiveDamage:
                    DamageDeduction += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.IgnoreDefend:
                    IgnoreDef += (int)ChangeValue;
                    break;
                case BattleBuffEventType_RolePropertyChange.DamageNumFluctuations:
                    //todo 伤害浮动加成暂不处理
                    break;
                case BattleBuffEventType_RolePropertyChange.HealEffect:
                    break;
            }
        }

        public BuffCharacterData(ICharacterBattleData roleBattleData)
        {
            this.roleBattleData = roleBattleData;
        }
    }
}
