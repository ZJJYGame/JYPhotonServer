using AscensionServer.Model;
using FluentNHibernate.Mapping;




namespace AscensionServer.DOMap
{
   public class VareityPurchaseRecordMap : ClassMap<VareityPurchaseRecord>
    {
        public VareityPurchaseRecordMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.VareityPurchasedCount).Column("vareity_purchased_count");
            Table("vareity_purchased_record");
        }
    }
}


