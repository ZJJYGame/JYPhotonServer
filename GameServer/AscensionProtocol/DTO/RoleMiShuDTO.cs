using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public  class RoleMiShuDTO:ProtocolDTO
    {
        public virtual int RoleId { get; set; }
        public virtual string MiShuIdArray { get; set; }

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
