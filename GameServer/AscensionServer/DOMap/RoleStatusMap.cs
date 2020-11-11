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
            Map(x => x.BestBlood).Column("best_blood");
            Map(x => x.BestBloodMax).Column("best_blood_max");
            Map(x => x.AttackPhysical).Column("attackphysical");
            Map(x => x.DefendPhysical).Column("defendphysical");
            Map(x => x.AttackPower).Column("attackpower");
            Map(x => x.DefendPower).Column("defendpower");
            Map(x => x.AttackSpeed).Column("attackspeed");
            Map(x => x.RoleSoul).Column("role_soul");
            Map(x => x.RoleMaxSoul).Column("role_max_soul");
            Map(x => x.ValueHide).Column("valuehide");
            Map(x => x.RolePopularity).Column("role_popularity");
            Map(x => x.RoleMaxPopularity).Column("role_max_popularity");
            Map(x => x.PhysicalCritProb).Column("physical_critprob");
            Map(x => x.MagicCritProb).Column("magic_critProb");
            Map(x => x.PhysicalCritDamage).Column("physical_crit_damage");
            Map(x => x.MagicCritDamage).Column("magic_crit_damage");
            Map(x => x.MoveSpeed).Column("move_speed"); 
            Table("role_status");
        }
    }
}
