using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class MonsterStatusMap:ClassMap<MonsterStatus>
    {
        public MonsterStatusMap()
        {
            Id(x => x.MonsterID).Column("monster_id").GeneratedBy.Increment();
            Map(x => x.MonsterMaxHP).Column("monster_max_hp");
            Map(x => x.MonsterHP).Column("monster_hp");
            Map(x => x.MonsterMaxMP).Column("monster_max_mp");
            Map(x => x.MonsterMP).Column("monster_mp");
            Map(x => x.MonsterAttackDamage).Column("monster_attack_damage");
            Map(x => x.MonsterResistanceAttack).Column("monster_resistance_attack");
            Map(x => x.MonsterPowerDamage).Column("monster_power_damage");
            Map(x => x.MonsterResistancePower).Column("monster_resistance_power");
            Map(x => x.MonsterSpeed).Column("monster_speed");
            Map(x => x.MonsterTalent).Column("monster_talent");
            Map(x => x.MonsterShenshi).Column("monster_shenshi");
            Map(x => x.MonsterShenhunDamage).Column("monster_shenhun_damage");
            Map(x => x.MonsterShenhunResistance).Column("monster_shenhun_resistance");
            Table("monster_status");
        }
    }
}
