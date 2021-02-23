using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 角色等级
    /// 煞气值
    /// 触发几率
    /// 心魔ID
    /// </summary>
    [Serializable]
    [ConfigData]
   public  class DemonData
    {
        public int Index { get; set; }
        public int Level_ID { get; set; }
        public List<int> Crary_Value { get; set; }
        public List<int> Trigger_Chance { get; set; }
        public List<int> Demon_ID { get; set; }
    }
}


