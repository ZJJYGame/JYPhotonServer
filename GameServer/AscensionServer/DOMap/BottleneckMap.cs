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
            Table("role_bottleneck");
        }
    }
}
