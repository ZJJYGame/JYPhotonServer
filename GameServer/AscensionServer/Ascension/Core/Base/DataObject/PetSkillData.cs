using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PetSkillData
    {
        public int SkillID { get; set; }
        public bool ISActiveSkill { get; set; }
        public List<int> AttributionType { get; set; }
        public List<int> Percentage { get; set; }
        public List<int> FixedValue { get; set; }
        public List<int> AddBBUFFID { get; set; }
    }
}
