using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class DemonicSoulMap: ClassMap<DemonicSoul>
    {
        public DemonicSoulMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.DemonicSouls).Column("demonicsouls");
            Map(x => x.DemonicSoulIndex).Column("demonicsouls_index");
            Table("demonicsoul");
        }
    }
}
