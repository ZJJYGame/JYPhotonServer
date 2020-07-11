using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.VO
{
    /// <summary>
    /// 数据类型是用来展示其他怪物信息的
    /// </summary>
    public class MonsterDataVO : ViewObject
    {
        public int MonsterGlobal { get; set; }
        public int MonsterID { get; set; }
        public string MonsterName { get; set; }
        public string HaemalStrand { get; set; }
        public short MonsterLevel { get; set; }
        public override void Clear()
        {
            MonsterGlobal = -1;
            MonsterGlobal = -1;
            MonsterName = null;
            HaemalStrand = null;
            MonsterLevel = -1;
        }
        public MonsterDataVO()
        {
            MonsterGlobal = -1;
            MonsterGlobal = -1;
            MonsterName = null;
            HaemalStrand = null;
            MonsterLevel = -1;
        }
    }
}
