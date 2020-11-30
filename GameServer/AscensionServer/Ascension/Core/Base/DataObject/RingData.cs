using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class RingData
    {
        public int Ring_ID { get; set; }
        public string Ring_Name { get; set; }
        public string Ring_Describe { get; set; }
        public string Ring_Icon { get; set; }
        public int Ring_Quality { get; set; }
        public int Need_Level_ID { get; set; }
        public int Ring_Number { get; set; }
        public EquipType Ring_Type { get; set; }
    }
}
