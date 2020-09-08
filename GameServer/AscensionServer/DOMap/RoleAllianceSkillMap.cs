using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionServer.Model;
using FluentNHibernate.Mapping;

namespace AscensionServer.DOMap
{
  public  class RoleAllianceSkillMap:ClassMap<RoleAllianceSkill>
    {
        public RoleAllianceSkillMap()
        {
            Id(x => x.RoleID).GeneratedBy.Assigned().Column("roleid");
            Map(x => x.SkillInsight).Column("skill_insight");
            Map(x => x.SkillMeditation).Column("skill_meditation");
            Map(x => x.SkillRapid).Column("skill_rapid");
            Map(x => x.SkillStrong).Column("skill_strong");
            Table("role_alliance_skill");
        }


    }
}
