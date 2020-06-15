using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    public class RoleTransformDTO:ProtocolDTO
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX{ get; set; }
        public float RotationY{ get; set; }
        public float RotationZ{ get; set; }
        public override void Clear()
        {
            PositionX = 0;
            PositionY = 0;
            PositionZ = 0;
            RotationX = 0;
            RotationY = 0;
            RotationZ = 0;
        }
    }
}
