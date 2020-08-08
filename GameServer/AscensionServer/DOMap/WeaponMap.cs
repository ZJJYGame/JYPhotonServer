﻿using System;
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
            Id(x => x.RoleID).Column("id").GeneratedBy.Assigned();
            Map(x => x.Weaponindex).Column("weapon_index");
            Map(x => x.WeaponStatusDict).Column("weapon_status_dict");
            Table("weapon");
        }
    }
}
