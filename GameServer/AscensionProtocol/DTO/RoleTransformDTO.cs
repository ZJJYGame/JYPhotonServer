using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class RoleTransformDTO : DataTransferObject
    {
        public void SetData( float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ)
        {
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
        }
        public int RoleID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            PositionX = 0;
            PositionY = 0;
            PositionZ = 0;
            RotationX = 0;
            RotationY = 0;
            RotationZ = 0;
        }
        public override string ToString()
        {
            return "###### RoleID : " + RoleID + " ; PositionX : " + PositionX
         + "; PositionY: " + PositionY + "; PositionZ : " + PositionZ
         + ";RotationX : " + RotationX + ";RotationY : " + RotationY + ";RotationZ: " + RotationZ +"######";
        }

    }
}
