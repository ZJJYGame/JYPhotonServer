using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class RepairPuppetData
    {
        public int PuppetID { get; set; }
        public List<int> RepairMaterials { get; set; }
        public List<int> MaterialsNumbers { get; set; }
    }
}
