using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 纯数据类
    /// </summary>
    [Serializable]
    public class Vector3DTO:DataTransferObject
    {
        public Vector3DTO(float posX, float posY, float posZ)
        {
            this.posX = posX;
            this.posY = posY;
            this.posZ = posZ;
        }
        public Vector3DTO() { }
        public virtual float posX { get; set; }
        public virtual float posY { get; set; }
        public virtual float posZ { get; set; }
        public override void Clear()
        {
            posX = 0;
            posY = 0;
            posZ = 0;
        }
    }
}
