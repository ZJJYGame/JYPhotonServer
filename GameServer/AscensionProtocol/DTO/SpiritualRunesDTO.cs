using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class SpiritualRunesDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int JobLevel { get; set; }
        public virtual HashSet<int> Recipe_Array { get; set; }


        public override void Clear()
        {
            RoleID = -1;
            JobLevel = 0;
            Recipe_Array = null;
        }
    }
}
