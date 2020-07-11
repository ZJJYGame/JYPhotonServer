using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 怪物的位置信息
    /// </summary>
    /// 
    [Serializable]
    public class MonsterTransformDTO : DataTransferObject
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

        public int MonsterGlobal { get; set; }
        public int MonsterID { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public override void Clear()
        {
            MonsterGlobal = -1;
            MonsterID = -1;
            PositionX = 0;
            PositionY = 0;
            PositionZ = 0;
            RotationX = 0;
            RotationY = 0;
            RotationZ = 0;
        }
    }
}
