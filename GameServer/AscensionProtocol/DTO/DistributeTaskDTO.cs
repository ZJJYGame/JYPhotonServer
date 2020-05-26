using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class DistributeTaskDTO : ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleTaskID { get; set; }
        public virtual string RoleTaskState { get; set; }
    }
}

