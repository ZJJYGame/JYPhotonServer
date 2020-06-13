using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    public class RoleDataDTO:ProtocolDTO
    {
        public RoleTransformDTO RoleTransform { get; set; }
        public  byte RoleFaction { get; set; }
        public  string RoleName { get; set; }
        public short GongFaLevel { get; set; }
    }
}
