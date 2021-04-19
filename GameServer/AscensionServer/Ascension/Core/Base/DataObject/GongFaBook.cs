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
         public int BookID { get; set; }
        public int BookProperty { get; set; }
        public int GongfaID { get; set; }
        public int NeedRoleLeve { get; set; }
        public int NeedGongfaID { get; set; }
    }
}


