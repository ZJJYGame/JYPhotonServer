using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AscensionProtocol;

namespace AscensionProtocol.DTO
{
    [Serializable]
   public class RoleAssetsDTO: DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int SpiritStonesLow { get; set; }
        public virtual int SpiritStonesMedium { get; set; }
        public virtual int SpiritStonesHigh { get; set; }
        public virtual int XianYu { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            SpiritStonesLow = 0;
            SpiritStonesMedium = 0;
            SpiritStonesHigh = 0;
            XianYu = 0;
        }
    }
}
