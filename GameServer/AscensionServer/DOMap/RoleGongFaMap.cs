using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class RoleGongFaMap:ClassMap<RoleGongFa>
    {
        public RoleGongFaMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.GongFaIDArray).Nullable().Column("gongfa_id_array");
            Table("role_gongfa");
        }
    }
}
