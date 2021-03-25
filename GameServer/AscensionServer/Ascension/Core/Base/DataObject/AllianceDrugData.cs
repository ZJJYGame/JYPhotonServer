using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public  class AllianceDrugData
    {
        public int DrugID { get; set; }
        public int SpiritStones { get; set; }
        public int AllianceContribution { get; set; }
        public int DrugHouseLevel { get; set; }
    }
}
