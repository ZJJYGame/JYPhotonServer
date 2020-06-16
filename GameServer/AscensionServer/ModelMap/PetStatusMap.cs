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
            Id(x => x.PetID).Column("pet_id").GeneratedBy.Assigned();
            Map(x => x.PetMaxHP).Column("pet_max_hp");
            Map(x => x.PetHP).Column("pet_hp");
            Map(x => x.PetMaxMP).Column("pet_max_mp");
            Map(x => x.PetMP).Column("pet_mp");
            Map(x => x.PetAttackDamage).Column("pet_attack_damage");
            Map(x => x.PetResistanceAttack).Column("pet_resistance_attack");
            Map(x => x.PetAbilityPower).Column("pet_ability_power");
            Map(x => x.PetResistancePower).Column("pet_resistance_power");
            Map(x => x.PetSpeed).Column("pet_speed");
            Map(x => x.PetTalent).Column("pet_talent");
            Map(x => x.PetShenhun).Column("pet_shenhun");
            Map(x => x.PetMaxShenhun).Column("pet_max_shenhun");
            Map(x => x.PetShenhunDamage).Column("pet_shenhun_damage");
            Map(x => x.PetShenhunResistance).Column("pet_shenhun_resistance");
            Table("pet_status");
        }
    }
}
