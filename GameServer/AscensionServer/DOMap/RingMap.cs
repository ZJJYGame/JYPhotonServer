using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RingMap:ClassMap<Ring>
    {
        public RingMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.RingId).Column("ring_id");
            Map(x => x.RingItems).Column("ring_items");
            Map(x => x.RingAdorn).Column("ring_adorn");
            Table("ring");
        }
    }
}
