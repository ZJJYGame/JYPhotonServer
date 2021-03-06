using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer.ModelMap
{
    public class OnOffLineMap : ClassMap<OnOffLine>
    {
        public OnOffLineMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.MsGfID).Column("gongfa_or_mishu");
            Map(x => x.ExpType).Column("exptype");
            Map(x => x.OffTime).Column("offtime");
            Table("onoffline");
        }
    }
}


