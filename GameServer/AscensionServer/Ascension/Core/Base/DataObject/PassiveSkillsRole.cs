using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PassiveSkillsRole
    {
        public int SkillID { get; set; }
        public List<int> Attribute { get; set; }
        public List<int> Percentage { get; set; }
        public List<int> Fixed { get; set; }
        public int WeaponType { get; set; }
    }
}
