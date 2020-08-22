using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class RoleAllianceMap: ClassMap<RoleAlliance>
    {
        
        public RoleAllianceMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.AllianceID).Column("alliance_id");
            Map(x => x.AllianceJob).Column("alliance_job");
            Map(x => x.Reputation).Column("reputation");
            Table("role_alliance");
        }
    }
}
