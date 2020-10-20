using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class BottleneckMap:ClassMap<Bottleneck>
    {
        public BottleneckMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.IsBottleneck).Column("isbottleneck");
            Map(x => x.IsThunder).Column("isthunder");
            Map(x => x.SpiritualRootVaule).Column("spiritualroot_vaule");
            Map(x => x.ThunderRound).Column("thunderround");
            Map(x => x.BreakThroughVaule).Column("breakthrough_vaule");
            Map(x => x.CraryVaule).Column("craryvaule");
            Map(x => x.DemonID).Column("demon_id");
            Map(x => x.IsDemon).Column("isdemon");
            Table("role_bottleneck");
        }
    }
}
