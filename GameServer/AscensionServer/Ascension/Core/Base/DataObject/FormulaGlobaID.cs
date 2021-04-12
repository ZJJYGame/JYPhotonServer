using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
   public class FormulaGlobaID
    {
        public int GlobalID { get; set; }
        public int ItemTypeDetail { get; set; }
    }
}
