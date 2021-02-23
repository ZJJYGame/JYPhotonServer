using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //瓶颈突破数据表
    /// <summary>
    /// 角色等级 
    /// 灵根数
    /// 天雷类型
    /// 雷劫回合数
    /// </summary>
    [Serializable]
    [ConfigData]
   public class BottleneckData
    {
        public bool IsFinalLevel { get; set; }
        public int Level_ID { get; set; }
        public List<int> Spiritual_Root_1 { get; set; }
        public List<int> Spiritual_Root_2 { get; set; }
        public List<int> Spiritual_Root_3 { get; set; }
        public List<int> Spiritual_Root_4 { get; set; }
        public List<int> Spiritual_Root_5 { get; set; }
        public int Thunder_ID { get; set; }
        public int Thunder_Round { get; set; }
    }
}


