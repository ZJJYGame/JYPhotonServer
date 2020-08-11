using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
   public class RolePurchaseRecordMap : ClassMap<RolePurchaseRecord>
    {
        public RolePurchaseRecordMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.GoodsPurchasedCount).Column("goods_purchased_count");
            Table("role_purchase_record");
        }
    }
}
