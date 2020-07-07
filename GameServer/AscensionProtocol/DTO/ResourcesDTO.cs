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
        public virtual string Name { get; set; }

        public virtual int Amout { get; set; }

        public virtual int Level { get; set; }

        public override void Clear()
        {
            ID = 0;
            Name = null;
            Amout = 0;
            Level = 0;
        }
    }
}
