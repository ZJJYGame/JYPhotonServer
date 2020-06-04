using System;
using System.Collections.Generic;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleTaskProgressDTO : ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleTaskInfo { get; set; }
    }
}

