using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PuppetAssembleData
    {
        public int PuppetID { get; set; }
        public int NeedVitality { get; set; }
        public int NeedMoney { get; set; }
        public int NeedRoleLevel { get; set; }
        public int NeedJobLevel { get; set; }
    }
}
