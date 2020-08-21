using AscensionServer.Model;
using FluentNHibernate.Mapping;



namespace AscensionServer.DOMap
{
   public class AllianceStatusMap: ClassMap<AllianceStatus>
    {
        public AllianceStatusMap()
        {
            Id(x => x.ID).Column("id").GeneratedBy.Assigned();
            Map(x => x.AlliancePeopleMax).Column("alliancepeoplemax");
            Map(x => x.AllianceLevel).Column("alliancelevel");
            Map(x => x.AllianceMaster).Column("alliancemaster");
            Map(x => x.AllianceName).Column("alliancename");
            Map(x => x.AllianceNumberPeople).Column("alliancenumberpeople");
            Map(x => x.Manifesto).Column("manifesto");
            Map(x => x.Popularity).Column("popularity");
            Table("alliancestatus");
        }

    }
}
