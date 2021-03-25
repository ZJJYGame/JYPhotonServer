using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class AllianceConstructionData
    {
        public byte BuildingType { get; set; }
        public List<ConstructionInfo> AllianceBuildingData { get; set; }
    }
    public class ConstructionInfo
    {
        public int BuildingLevel { get; set; }
        public int NeedAllianceSpiritStones { get; set; }
        public int DailyFeeSpiritStones { get; set; }
        public int DailyFee { get; set; }
        public int MaxValue { get; set; }
    }
}
