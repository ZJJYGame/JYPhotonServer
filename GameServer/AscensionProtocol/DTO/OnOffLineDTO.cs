using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
   public class OnOffLineDTO:ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual int UpgradeExp { get; set; }
        public virtual string OffLineTime { get; set; }
    }
}
