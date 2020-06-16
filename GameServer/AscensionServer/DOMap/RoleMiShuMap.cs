using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer
{
    public class RoleMiShuMap:ClassMap<RoleMiShu>
    {
        public RoleMiShuMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.MiShuIDArray).Column("mishu_id_array");
            Table("role_mishu");
        }
    }
}
