using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
   public  class HerbsFieldMap:ClassMap<HerbsField>
    {
        public HerbsFieldMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("id");
            Map(x => x.jobLevel).Column("job_level");
            Map(x => x.AllHerbs).Column("all_herbs");
            Table("herbsfield");
        }
    }
}
