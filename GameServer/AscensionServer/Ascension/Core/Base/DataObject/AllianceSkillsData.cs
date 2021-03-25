using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public  class AllianceSkillsData
    {
        public string GangsSkillType { get; set; }
        public List<SkillsData> AllianceSkillData { get; set; }
    }
    [Serializable]
    [ConfigData]
    public class SkillsData
    {
        public string SkillType { get; set; }
        public int SkillLevel { get; set; }
        public int AddCoefficient { get; set; }
        public int SpiritStones { get; set; }
        public int AllianceContribution { get; set; }
    }

}


