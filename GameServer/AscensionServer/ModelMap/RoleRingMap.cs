using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleRingMap:ClassMap<RoleRing>
    {
        public RoleRingMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.RingID).Column("ring_id").Nullable();
            Table("role_ring");
        }
    }
}
