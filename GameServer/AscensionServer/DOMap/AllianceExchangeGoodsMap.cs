using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer.DOMap
{
    class AllianceExchangeGoodsMap : ClassMap<AllianceExchangeGoods>
    {
        public AllianceExchangeGoodsMap()
        {
            Id(x => x.AllianceID).GeneratedBy.Assigned().Column("allianceid");
            Map(x => x.ExchangeGoods).Nullable().Column("exchange_goods");
            Table("alliance_exchange_goods");
        }
    }
}
