using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
  public  class AllianceConstructionMap: ClassMap<AllianceConstruction>
    {
        public AllianceConstructionMap()
        {
            Id(x => x.AllianceID).GeneratedBy.Assigned().Column("allianceid");
            Map(x => x.AllianceAlchemyStorage).Nullable().Column("alliance_alchemy_storage");
            Map(x => x.AllianceAssets).Nullable().Column("alliance_assets");
            Map(x => x.AllianceArmsDrillSite).Nullable().Column("alliance_drillsite");
            Map(x => x.AllianceChamber).Nullable().Column("alliance_chamber");
            Map(x => x.AllianceScripturesPlatform).Nullable().Column("alliance_scriptures_platform");
            Table("alliance_construction");
        }
    }
}


