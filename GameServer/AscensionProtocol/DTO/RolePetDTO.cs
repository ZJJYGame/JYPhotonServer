using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RolePetDTO:ProtocolDTO
    {
        public virtual int RoleID { get; set; }
        public virtual string PetIDArray { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            PetIDArray = null;
        }
    }
}
