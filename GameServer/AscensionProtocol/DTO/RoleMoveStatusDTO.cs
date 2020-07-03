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
        public string Default { get; set; }
        public string FlyCarrier { get; set; }
        public string FlyShip { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            Default = "";
            FlyCarrier = "";
            FlyShip = "";
        }
    }
}
