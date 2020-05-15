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
            Map(x => x.WeaponExp).Column("weapon_exp");
            Map(x => x.WeaponID).Column("weapon_id");
            Map(x => x.WeaponLevel).Column("weapon_level");
            Table("weapon");
        }
    }
}
