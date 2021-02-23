using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer
{
    public class RoleEquipMap:ClassMap<RoleEquip>
    {
        public RoleEquipMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.Equips).Column("equips");
            Table("role_equip");
        }
    }
}


