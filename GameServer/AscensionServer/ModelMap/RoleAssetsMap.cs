using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer.ModelMap
{
    public class RoleAssetsMap:ClassMap<RoleAssets>
    {
        public RoleAssetsMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.SpiritStonesLow).Column("spirit_stones_low");
            Map(x => x.SpiritStonesMedium).Column("spirit_stones_medium");
            Map(x => x.SpiritStonesHigh).Column("spirit_stones_high");
            Map(x => x.XianXu).Column("xianxu");
            Table("role_assets");
        }
    }
}
