using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleGongFaDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int,int> GongFaIDArray { get; set; }
        public RoleGongFaDTO()
            {
            GongFaIDArray = new Dictionary<int, int>();
        }
        public override void Clear()
        {
            RoleID = -1;
            GongFaIDArray = null;
        }
    }
}
