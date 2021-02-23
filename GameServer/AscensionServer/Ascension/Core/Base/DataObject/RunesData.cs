using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 针对符箓实体类
    /// </summary>
    [Serializable]
    [ConfigData]
    public class RunesData
    {
        public int Runes_ID { get; set; }
        public string Runes_Name { get; set; }
        public string Runes_Describe { get; set; }
        public string Runes_Icon { get; set; }
        public bool IsBattle { get; set; }
        public int Runes_Skill { get; set; }
        public int Need_Level_ID { get; set; }
        public string Animation_Path { get; set; }
        public string Runes_Effect { get; set; }
        public int Usage_Count { get; set; }

    }
 
}


