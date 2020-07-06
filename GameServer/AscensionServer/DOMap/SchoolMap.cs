using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class SchoolMap : ClassMap<School>
    {
        public SchoolMap()
        {
            Id(x => x.ID).GeneratedBy.Assigned().Column("id");
            Map(x => x.RankingListID).Column("ranking_list_id");
            Map(x => x.SchoolID).Column("school_id");
            Map(x => x.SchoolJob).Column("school_job");
            Map(x => x.SchoolMember).Column("school_member");
            Map(x => x.SutrasAtticID).Column("sutras_attic_id");
            Map(x => x.TreasureAtticID).Column("treasure_attic_id");
            Map(x => x.Hatred).Column("hatred");
            Map(x => x.Contribution).Column("contribution");
            Table("school");
        }
    }
}
