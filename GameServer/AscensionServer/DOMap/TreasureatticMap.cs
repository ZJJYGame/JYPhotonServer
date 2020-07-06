using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class TreasureatticMap : ClassMap<Treasureattic>
    {
        public TreasureatticMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.ItemAmountDict).Column("item_amount_dict");
            Map(x => x.ItemRedeemedDict).Column("item_redeemed_dict");
            Table("treasureattic");

        }
    }
}
