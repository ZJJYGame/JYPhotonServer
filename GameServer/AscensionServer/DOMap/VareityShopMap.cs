using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class VareityShopMap : ClassMap<VareityShop>
    {
        public VareityShopMap()
        {
            Id(x => x.VareityshopID).GeneratedBy.Increment().Column("id");
            Map(x => x.AllGoods).Column("ranking_list_id");
        }
    }
}
