using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public class RoleStatusPointMap : ClassMap<RoleStatusPoint>
    {
        public RoleStatusPointMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.SlnNow).Column("slnnow");
            Map(x => x.AbilityPointSln).Column("ability_point_sln");
            Table("role_ability_point");
        }

    }
}
