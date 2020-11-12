using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public  class AllianceSkillData
    {
        public string GangsSkillType { get; set; }
        public List<AllianceSkillsData> allianceSkillsDatas { get; set; }
    }
    [Serializable]
    [ConfigData]
    public class AllianceSkillsData
    {
        public string Skill_Type { get; set; }
        public int Skill_Level { get; set; }
        public int AddCoefficient { get; set; }
    }

}
