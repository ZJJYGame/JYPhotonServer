using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class WeaponMap:ClassMap<Weapon>
    {
        public WeaponMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Increment();
            Map(x => x.WeaponID).Column("weapon_id");
            Map(x => x.WeaponPart).Column("weapon_part");
            Map(x => x.WeaponLevel).Column("weapon_level");
            Map(x => x.WeaponHP).Column("weapon_hp");
            Map(x => x.WeaponSpeed).Column("weapon_speed");
            Map(x => x.WeaponAttackDamage).Column("weapon_attack_damage");
            Map(x => x.WeaponResistanceDamage).Column("weapon_resistance_damage");
            Map(x => x.WeaponAttackPower).Column("weapon_attack_power");
            Map(x => x.WeaponResistancePower).Column("weapon_resistance_power");
            Table("weapon");
        }
    }
}
