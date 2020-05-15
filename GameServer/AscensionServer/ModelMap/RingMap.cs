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
            Map(x => x.ItemArray).Column("item_array");
            Map(x => x.RingID).Column("ring_id");
            Table("ring");
        }
    }
}
