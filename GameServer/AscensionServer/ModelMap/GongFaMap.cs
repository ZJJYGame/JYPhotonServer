using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class GongFaMap:ClassMap<GongFa>
    {
        public GongFaMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.SkillArray).Column("skill_array").Nullable();
            Table("gongfa");
        }
    }
}
