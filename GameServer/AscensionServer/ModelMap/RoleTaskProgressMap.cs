using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer.ModelMap
{
    class RoleTaskProgressMap:ClassMap<RoleTaskProgress>
    {
        public RoleTaskProgressMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.RoleTaskID).Column("role_task_id");
            Table("role_task_progress");
        }
    }
}
