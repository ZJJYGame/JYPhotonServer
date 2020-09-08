using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleAllianceSkill : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual int SkillStrong { get; set; }
        public virtual int SkillInsight { get; set; }
        public virtual int SkillMeditation { get; set; }
        public virtual int SkillRapid { get; set; }

        public RoleAllianceSkill()
        {
            SkillStrong =1;
            SkillInsight =1;
            SkillMeditation = 1;
            SkillRapid =1;
        }

        public override void Clear()
        {
            RoleID = -1;
            SkillStrong = 0;
            SkillInsight = 0;
            SkillMeditation = 0;
            SkillRapid = 0;

        }
    }
}
