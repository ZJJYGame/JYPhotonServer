using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AscensionProtocol.DTO
{
    public class RoleDataDTO:ProtocolDTO
    {
        public string RoleTransformJson { get; set; }
        public  byte RoleFaction { get; set; }
        public  string RoleName { get; set; }
        public short GongFaLevel { get; set; }
        public override void Clear()
        {
            RoleTransformJson = null;
            RoleFaction = 0;
            RoleName = null;
            GongFaLevel = 0;
        }
    }
}
