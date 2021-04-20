using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public class PassiveSkillsWeapon : IPassiveSkills
    {
        public int SkillID { get; set; }
        public List<int> Attribute { get; set; }
        public List<int> Percentage { get; set; }
        public List<int> Fixed { get; set; }
        /// <summary>
        /// 0是人物加成，1是武器加成
        /// </summary>
        public int AddTarget { get; set; }
    }
}
