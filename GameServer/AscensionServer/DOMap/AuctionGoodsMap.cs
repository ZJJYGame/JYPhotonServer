using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class AuctionGoodsMap:ClassMap<AuctionGoods>
    {
        public AuctionGoodsMap()
        {
            Id(x => x.RedisKey).GeneratedBy.Assigned().Column("redisKey");
            Map(x => x.RoleID).Column("roleID");
            Map(x => x.GlobalID).Column("globalID");
            Map(x => x.Price).Column("price");
            Map(x => x.ItemData).Column("itemData");
            Map(x => x.Count).Column("count");
        }
    }
}
