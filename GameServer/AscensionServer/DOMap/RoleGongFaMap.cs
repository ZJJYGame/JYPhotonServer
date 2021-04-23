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
            Map(x => x.GongFaIDDict).Nullable().Column("gongfa_id_dict");
            Table("role_gongfa");
        }
    }
}


