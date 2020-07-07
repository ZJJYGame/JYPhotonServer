using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class RoleSchoolMap : ClassMap<RoleSchool>
    {
        public RoleSchoolMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.RoleJoinedSchool).Column("role_joined_school");
            Map(x => x.RoleJoiningSchool).Column("role_joining_school");
            Table("role_school");
        }
    }
}
