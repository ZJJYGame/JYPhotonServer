using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleGongFaDTO : ProtocolDTO
    {
        public virtual int RoleId { get; set; }
        public virtual string GongFaIdArray { get; set; }
    }
}
