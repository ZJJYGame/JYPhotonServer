using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
  public  class RoleEquipmentMap : ClassMap<RoleEquipment>
    {
        public RoleEquipmentMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.Weapon).Column("weapon");
            Map(x => x.MagicWeapon).Column("magic_weapon");
            Table("role_equipment");

        }

    }
}
