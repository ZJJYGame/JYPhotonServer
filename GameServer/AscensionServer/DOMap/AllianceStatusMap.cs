using AscensionServer.Model;
using FluentNHibernate.Mapping;



namespace AscensionServer.DOMap
{
   public class AllianceStatusMap: ClassMap<AllianceStatus>
    {
        public AllianceStatusMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Increment();
            Map(x => x.AlliancePeopleMax).Column("alliance_people_max");
            Map(x => x.AllianceLevel).Column("alliance_level");
            Map(x => x.AllianceMaster).Column("alliance_master");
            Map(x => x.AllianceName).Column("alliance_name");
            Map(x => x.AllianceNumberPeople).Column("alliance_number_people");
            Map(x => x.Manifesto).Column("manifesto");
            Map(x => x.Popularity).Column("popularity");
            Table("alliancestatus");
        }

    }
}
