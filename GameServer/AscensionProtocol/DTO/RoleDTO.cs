using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleDTO:ProtocolDTO
    {
        public virtual int RoleId { get; set; }
        public virtual byte RoleFaction { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string RoleRoot { get; set; }
        public virtual int RoleLevel { get; set; }
        public virtual int RoleExp { get; set; }
        public virtual byte RoleGender { get; set; }

        public override void Clear()
        {
            RoleId = 0;
            RoleFaction = 0;
            RoleName = null;
            RoleRoot = null;
            RoleLevel = 0;
            RoleExp = 0;
            RoleGender = 0;
        }
    }
}
