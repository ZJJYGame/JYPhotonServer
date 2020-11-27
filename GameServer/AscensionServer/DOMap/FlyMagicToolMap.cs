using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class FlyMagicToolMap : ClassMap<FlyMagicTool>
    {
        public FlyMagicToolMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.AllFlyMagicTool).Column("all_fly_magic_tool");
            Map(x => x.FlyToolLayoutDict).Column("all_fly_magic_tool");
            Table("role_fly_magic_tool");
        }
    }
}
