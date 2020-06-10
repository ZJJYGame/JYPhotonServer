using System;
using System.Collections.Generic;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleTaskProgressDTO : ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int,RoleTaskItemDTO> RoleTaskInfoDic { get; set; }
    }
    [Serializable]
    public class RoleTaskItemDTO
    {
        public virtual string RoleTaskState  { get; set; }
        public virtual string RoleTaskType { get; set; }
        public virtual string RoleTaskResult{ get; set; }
    }
}

