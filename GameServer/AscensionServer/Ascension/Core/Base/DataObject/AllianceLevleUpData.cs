using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 建筑等级
    /// 升级扩充人数
    /// 每人维护消耗灵石
    /// 每日维护消耗人气
    /// </summary>
    [Serializable]
    [ConfigData]
   public class AllianceLevleUpData
    {
        public int Building_Level { get; set; }
        public int LevelUp_Describe { get; set; }
        public int Daily_Fee_Spirit_Stones { get; set; }
        public int Daily_Fee { get; set; }

    }
}
