using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class ResourcesDTO : DataTransferObject
    {

        public virtual int ID { get; set; }

        public virtual int Amout { get; set; }

        public virtual int Level { get; set; }

        public virtual Vector3DTO Vector3 { get; set; }
        public override void Clear()
        {
            ID = 0;
            Amout = 0;
            Level = 0;
            Vector3 = new Vector3DTO() { posX = 0, posY = 0, posZ = 0 };
        }

        [Serializable]
        public class Vector3DTO
        {
            public virtual float posX { get; set; } 
            public virtual float posY { get; set; } 
            public virtual float posZ { get; set; } 
        }
    }
    [Serializable]
    public class Vector2DTO
    {
        public virtual float posX { get; set; }
        public virtual float posY { get; set; }
    }
}
