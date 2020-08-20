using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class AlliancesMap : ClassMap<Alliances>
    {
        public AlliancesMap()
        {
            Id(x => x.ID).GeneratedBy.Assigned().Column("id");
            Map(x => x.AllianceList).Column("alliance_list");
            Table("alliances");
        }

    }
}
