using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer
{
    public class AlchemyMap:ClassMap<Alchemy>
    {
        public AlchemyMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.JobLevel).Column("joblevel");
            Map(x => x.Recipe_Array).Column("recipe_array");
            Table("alchemy");
        }
    }
}
