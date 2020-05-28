using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class RoleWeaponMap:ClassMap<RoleWeapon>
    {
        public RoleWeaponMap()
        {
            Id(x => x.RoleID).Column("role_id").GeneratedBy.Assigned();
            Map(x => x.WeaponIDAttay).Column("weapon_id_array");
            Table("role_weapon");
        }
    }
}
