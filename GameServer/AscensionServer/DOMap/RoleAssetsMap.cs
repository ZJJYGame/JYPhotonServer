using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer
{
    public class RoleAssetsMap:ClassMap<RoleAssets>
    {
        public RoleAssetsMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.SpiritStonesLow).Column("spirit_stones_low");
            Map(x => x.XianYu).Column("xianyu");
            Table("role_assets");
        }
    }
}
