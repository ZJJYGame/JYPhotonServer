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
            Id(x => x.RoleID).GeneratedBy.Increment().Column("roleid");
            Map(x => x.DemonicSouls).Column("demonicsouls").Nullable();
            Map(x => x.DemonicSoulIndex).Column("demonicsouls_index").Nullable();
            Table("demonicsoul");
        }
    }
}
