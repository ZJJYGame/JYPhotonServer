using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class AuctionIndexMap:ClassMap<AuctionIndex>
    {
        public AuctionIndexMap()
        {
            Id(x => x.GlobalID).GeneratedBy.Assigned().Column("globalid");
            Map(x => x.AuctionGoodsIndexList).Column("goods");
            Table("auction_putaway");
        }
    }
}


