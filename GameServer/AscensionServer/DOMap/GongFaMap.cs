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
            Map(x => x.GongFaID).Column("gongfa_id");
            Map(x => x.GongFaExp).Column("gongfa_exp");
            Map(x => x.GongFaLevel).Column("gongfa_level");
            Map(x => x.GongFaSkillArray).Column("gongfa_skill_array").Nullable();
            Table("gongfa");
        }
    }
}
