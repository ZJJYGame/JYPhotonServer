using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer {
    public class PuppetMap : ClassMap<Puppet>
    {
        public PuppetMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.JobLevel).Column("job_level");
            Map(x => x.Recipe_Array).Column("recipe_array");
            Map(x => x.JobLevelExp).Column("job_level_exp");
            Table("puppet");
        }
    }
}


