using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionProtocol;

namespace AscensionProtocol.DTO
{
    [Serializable]
   public class RoleAssetsDTO:ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual int SpiritStonesLow { get; set; }
        public virtual int SpiritStonesMedium { get; set; }
        public virtual int SpiritStonesHigh { get; set; }
        public virtual int XianYu { get; set; }
    }
}
