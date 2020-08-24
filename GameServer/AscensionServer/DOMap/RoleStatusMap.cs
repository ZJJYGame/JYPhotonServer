using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleStatusMap:ClassMap<RoleStatus>
    {
        public RoleStatusMap()
        {
            Id(x=>x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.RoleMaxHP).Column("role_max_hp");
            Map(x => x.RoleHP).Column("role_hp");
            Map(x => x.RoleMaxMP).Column("role_max_mp");
            Map(x => x.RoleMP).Column("role_mp");
            Map(x => x.RoleJingXue).Column("role_jingxue");
            Map(x => x.RoleAttackDamage).Column("role_attack_damage");
            Map(x => x.RoleResistanceDamage).Column("role_resistance_damage");
            Map(x => x.RoleAttackPower).Column("role_attack_power");
            Map(x => x.RoleResistancePower).Column("role_resistance_power");
            Map(x => x.RoleSpeedAttack).Column("role_speed_attack");
            Map(x => x.RoleShenhun).Column("role_shenhun");
            Map(x => x.RoleMaxShenhun).Column("role_max_shenhun");
            Map(x => x.RoleShenHunDamage).Column("role_shenhun_damage");
            Map(x => x.RoleShenHunResistance).Column("role_shenhun_resistance");
            Map(x => x.RoleCrit).Column("role_crit");
            Map(x => x.RoleCritResistance).Column("role_crit_resistance");
            Map(x => x.RoleDormant).Column("role_dormant");
            Map(x => x.RoleVileSpawn).Column("role_vilespawn");
            Map(x => x.RoleVitality).Column("role_viltality");
            Map(x => x.RoleKillingIntent).Column("role_killingintent");
            Table("role_status");
        }
    }
}
