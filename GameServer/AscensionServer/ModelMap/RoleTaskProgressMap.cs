using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer
{
public   class RoleTaskProgressMap:ClassMap<RoleTaskProgress>
    {
        public RoleTaskProgressMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.RoleTaskID).Column("role_task_id");
            Map(x => x.RoleTaskState).Column("role_task_state");
            Table("role_task_progress");
        }
    }
}
