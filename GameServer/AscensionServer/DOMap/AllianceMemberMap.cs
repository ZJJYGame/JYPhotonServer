using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class AllianceMemberMap:ClassMap<AllianceMember>
    {
        public AllianceMemberMap()
        {
            Id(x => x.AllianceID).GeneratedBy.Assigned().Column("allianceid");
            Map(x => x.ApplyforMember).Nullable().Column("applyfor_member");
            Map(x => x.Member).Nullable().Column("member");
            Table("alliance_member");
        }



    }
}
