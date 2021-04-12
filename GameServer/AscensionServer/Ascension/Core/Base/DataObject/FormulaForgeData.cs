using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class FormulaForgeData
    {
        public int FormulaID { get; set; }
        public List<int> NeedItemArray { get; set; }
        public List<int> NeedItemNumber { get; set; }
        public int ItemID { get; set; }
        public int SuccessRate { get; set; }
        public int NeedMoney { get; set; }
        public int NeedVitality { get; set; }
        public int MasteryValue { get; set; }
        public int FormulaLevel { get; set; }
        public int NeedJobLevel { get; set; }
        public int SyntheticType { get; set; }
    }
}
