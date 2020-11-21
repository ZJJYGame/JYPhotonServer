using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class PetStatusMap:ClassMap<PetStatus>
    {
        public PetStatusMap()
        {
            Id(x => x.PetID).GeneratedBy.Assigned().Column("pet_id");
            Map(x => x.PetMaxHP).Column("pet_max_hp");
            Map(x => x.PetHP).Column("pet_hp");
            Map(x => x.PetMaxMP).Column("pet_max_mp");
            Map(x => x.PetMP).Column("pet_mp");
            Map(x => x.PetShenhun).Column("pet_shenhun");
            Map(x => x.PetMaxShenhun).Column("pet_max_shenhun");
            Map(x => x.AttackSpeed).Column("attack_speed");
            Map(x => x.AttackPhysical).Column("attack_physical");
            Map(x => x.DefendPhysical).Column("defend_physical");
            Map(x => x.AttackPower).Column("attack_power");
            Map(x => x.DefendPower).Column("defend_power");
            Map(x => x.PhysicalCritProb).Column("physical_critProb");
            Map(x => x.MagicCritProb).Column("magic_critProb");
            Map(x => x.ReduceCritProb).Column("reduce_critProb");
            Map(x => x.PhysicalCritDamage).Column("physical_crit_damage");
            Map(x => x.MagicCritDamage).Column("magic_crit_damage");
            Map(x => x.ReduceCritDamage).Column("reduce_crit_damage");
            Map(x => x.ExpLevelUp).Column("exp_level_up");
            Table("pet_status");
        }
    }
}
