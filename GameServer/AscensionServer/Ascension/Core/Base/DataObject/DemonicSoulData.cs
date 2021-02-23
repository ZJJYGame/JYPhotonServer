using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class DemonicSoulData
    {
        public int DemonicSoulID { get; set; }

        public List<List<int>> DemonicSoulSkill { get; set; }

        public List<int> MergeNumber { get; set; }
        public int MergeSuccessRate { get; set; }
        public int MergeDemonicSoulID { get; set; }

    }
}


