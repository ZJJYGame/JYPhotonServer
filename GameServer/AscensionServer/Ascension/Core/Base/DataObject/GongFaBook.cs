using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /*功法书的实体类*/
    [Serializable]
    [ConfigData]
    public class GongFaBook 
    {
         public int Book_ID { get; set; }
        public string Book_Name { get; set; }
        public string Book_Describe { get; set; }
        public string Book_Icon { get; set; }
        public int Book_Quarity { get; set; }
        public int Book_Property { get; set; }
        public int Need_School_ID { get; set; }
        public int Need_Gongfa_ID { get; set; }
        public short Need_Level_ID { get; set; }
        public int Gongfa_ID { get; set; }
    }
}


