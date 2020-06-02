using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleTaskProgressDTO : ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<string, RoleTaskModel> RoleTaskDict { get; set; }
    }
    [Serializable]
    public class RoleTaskModel
    {
        public virtual string TaskState { get; set; }
        public virtual string TaskResult { get; set; }
        public virtual string TaskType { get; set; }
    }
}

