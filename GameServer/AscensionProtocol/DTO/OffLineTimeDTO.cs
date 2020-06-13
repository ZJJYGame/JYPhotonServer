using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
        [Serializable]
        public class OffLineTimeDTO : ProtocolDTO
        {
            public virtual int RoleID { get; set; }
            public virtual string OffTime { get; set; }
    }
}
