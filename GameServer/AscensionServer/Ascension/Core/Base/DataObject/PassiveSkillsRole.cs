using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PassiveSkillsRole: IPassiveSkills
    {
        public int SkillID { get; set; }
        public List<int> Attribute { get; set; }
        public List<int> Percentage { get; set; }
        public List<int> Fixed { get; set; }
        public int WeaponType { get; set; }
    }

    public interface IPassiveSkills
    {
        int SkillID { get; set; }
        List<int> Attribute { get; set; }
        List<int> Percentage { get; set; }
        List<int> Fixed { get; set; }
    }
}
