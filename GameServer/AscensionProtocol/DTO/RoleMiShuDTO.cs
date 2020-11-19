using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public  class RoleMiShuDTO: DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int,int> MiShuIDArray { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            MiShuIDArray = null;
        }

    }
}
