using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleAssets
    {
        public virtual int RoleID { get; set; }
        public virtual long SpiritStonesLow { get; set; }
        public virtual long SpiritStonesMedium { get; set; }
        public virtual long SpiritStonesHigh { get; set; }
        public virtual long XianXu { get; set; }
    }
}
