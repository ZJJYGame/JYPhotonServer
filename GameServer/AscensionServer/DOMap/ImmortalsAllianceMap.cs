using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
    public class ImmortalsAllianceMap: ClassMap<ImmortalsAlliance>
    {
        public ImmortalsAllianceMap()
        {
            Id(x => x.RoleID).Column("id").GeneratedBy.Assigned();
            Map(x => x.allianceStatus).Column("alliancestatus");
            Map(x => x.AllianceManifesto).Column("alliancemanifesto");
            Map(x => x.IsMaster).Column("ismaster");
            Map(x => x.AllianceName).Column("ismaster");
            Table("immortalsalliance");
        }
    }
}
