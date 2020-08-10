using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class ShoppingMallMap: ClassMap<ShoppingMall>
    {
        public ShoppingMallMap()
        {
            Id(x => x.ID).GeneratedBy.Assigned().Column("id");
            Map(x => x.Materials).Column("materials");
            Map(x => x.NewArrival).Column("newarrival");
            Map(x => x.QualifiedToBuy).Column("qualifiedtobuy");
            Table("shoppingmall");
        }
    }
}
