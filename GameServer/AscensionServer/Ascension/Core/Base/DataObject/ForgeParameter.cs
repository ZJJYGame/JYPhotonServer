using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 锻造随机属性值技能的Json数据类型
    /// </summary>
    [Serializable]
    [ConfigData]
    public class ForgeParameter
    {
        public int WeaponID { get; set; }
        public int WeaponQuality { get; set; }
        public int NeedLevelID { get; set; }
        public List<int> WeaponAttributeMin { get; set; }
        public List<int> WeaponAttributeMax { get; set; }
        public int WeaponDurable { get; set; }
        public List<int> SkillProbability { get; set; }
        public List<int> WeaponSkill { get; set; }
        public int WeaponType { get; set; }
    }
}
