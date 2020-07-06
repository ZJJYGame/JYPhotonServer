using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class RoleMoveStatusDTO : DataTransferObject
    {

        public int RoleID { get; set; }
        public string MoveStatus { get; set; }
        
        public override void Clear()
        {
            RoleID = -1;
            MoveStatus = "";
        }
    }
}
