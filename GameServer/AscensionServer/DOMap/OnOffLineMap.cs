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
            Map(x => x.GongFaExp).Column("gongfaexp");
            Map(x => x.OffLineTime).Column("offlinetime");
            Map(x => x.MiShuExp).Column("mishuexp");
            Map(x => x.MsGfID).Column("gongfa_or_mishu");
            Map(x => x.ExpType).Column("exptype");
            Table("onoffline");
        }
    }
}
