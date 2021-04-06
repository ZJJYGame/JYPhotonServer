﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class WeaponMap:ClassMap<RoleWeapon>
    {
        public WeaponMap()
        {
            Id(x => x.RoleID).Column("roleid").GeneratedBy.Assigned();
            Map(x => x.Weaponindex).Column("weapon_index");
            Map(x => x.WeaponStatusDict).Column("weapon_status_dict");
            Map(x => x.Magicindex).Column("magic_index");
            Map(x => x.MagicStatusDict).Column("magic_status_dict");
            Table("role_weapon");
        }
    }
}


