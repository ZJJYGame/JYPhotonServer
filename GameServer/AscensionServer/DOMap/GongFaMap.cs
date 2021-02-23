using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using FluentNHibernate.Mapping;
namespace AscensionServer
{
    public class GongFaMap:ClassMap<CultivationMethod>
    {
        public GongFaMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.CultivationMethodID).Column("cultivation_method_id");
            Map(x => x.CultivationMethodExp).Column("cultivation_method_exp");
            Map(x => x.CultivationMethodLevel).Column("cultivation_method_level");
            Map(x => x.CultivationMethodLevelSkillArray).Column("cultivation_method_skill_array").Nullable();
            Table("cultivation_method");
        }
    }
}


