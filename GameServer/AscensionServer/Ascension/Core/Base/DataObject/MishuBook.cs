using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class MishuBook
    {
        public int Book_ID { get; set; }
        public int Book_Quarity { get; set; }
        public int Book_Property { get; set; }
        public int Need_School_ID { get; set; }
        public int Need_Gongfa_ID { get; set; }
        public int Need_Level_ID { get; set; }
        public int Mishu_ID { get; set; }
    }
}


