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
            Map(x => x.RingItemId).Column("ring_item_id");
            Map(x => x.RingItemCount).Column("ring_item_count");
            Map(x => x.RingItemAdorn).Column("item_item_adorn");
            Table("ring");
        }
    }
}
