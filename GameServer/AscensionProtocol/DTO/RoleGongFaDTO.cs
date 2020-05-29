using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleGongFaDTO : ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual string GongFaIDArray { get; set; }
    }
}
