using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;

namespace AscensionServer
{
  public  class RoleRepositoryMap:ClassMap<RoleRepository>
    {
        public RoleRepositoryMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("role_id");
            Map(x => x.Items).Column("items");
            Table("role_repository");
        }
    }
}


