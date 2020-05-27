using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using AscensionServer.Model;
namespace AscensionServer.ModelMap
{
    public  class MiShuMap:ClassMap<MiShu>
    {
        public MiShuMap()
        {
            Id(x => x.ID).GeneratedBy.Increment().Column("id");
            Map(x => x.MiShuExp).Column("mishu_exp");
            Map(x => x.MiShuID).Column("mishu_id");
            Map(x => x.MiShuLevel).Column("mishu_level");
            Map(x => x.SkillArry).Column("skill_array");
            Table("mishu");
        }
    }
}
