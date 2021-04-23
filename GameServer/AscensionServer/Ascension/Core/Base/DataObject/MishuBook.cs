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
        public int BookID { get; set; }
        public int BookProperty { get; set; }
        public int MishuID { get; set; }
        public int NeedRoleLevel { get; set; }
    }
}


