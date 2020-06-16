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
        public virtual int GongFaExp { get; set; }
        public virtual string OffLineTime { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual int MsGfID { get; set; }
        public virtual int ExpType { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            GongFaExp = 0;
            OffLineTime = null;
            MiShuExp = 0;
            MsGfID = 0;
        }
    }
}
