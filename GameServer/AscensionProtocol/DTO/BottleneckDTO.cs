using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class BottleneckDTO : DataTransferObject
    {
       public virtual int RoleID { get; set; }
        public virtual bool IsBottleneck { get; set; }
    public override void Clear()
        {
            RoleID = -1;
            IsBottleneck = false;
        }
    }
}
