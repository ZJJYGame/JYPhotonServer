using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 纯位置信息DTO，用以传输位置数据
    /// </summary>
    [Serializable]
    public class TransformDTO : DataTransferObject
    {
        public void SetData(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ)
        {
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
        }
        public void SetDataMilVezes(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ)
        {
            PositionX = positionX*1000;
            PositionY = positionY *1000;
            PositionZ = positionZ * 1000;
            RotationX = rotationX * 1000;
            RotationY = rotationY * 1000;
            RotationZ = rotationZ * 1000;
        }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
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
