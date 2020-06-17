using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleRingDTO: DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual string RingIdArray { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            RingIdArray = null;
        }
    }
}
