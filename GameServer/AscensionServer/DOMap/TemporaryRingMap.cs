using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class TemporaryRingMap:ClassMap<TemporaryRing>
    {
        public TemporaryRingMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.RingItems).Column("ring_items");
            Table("temporary_ring");
        }
    }
}
