using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class SpiritRangeData
    {
        public int SpiritRangeID { get; set; }
        public int SpiritRangeLevel { get; set; }
        public List<int> Location { get; set; }
        public List<int> DongFuLevel { get; set; }
        public List<int> DongFuNum { get; set; }
    }
}
 