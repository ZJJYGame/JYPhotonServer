using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    [Obsolete("弃用，有新方法")]
    public class Vector3DataDTO: ProtocolDTO
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public override void Clear()
        {
            x = 0;
            y = 0;
            z = 0;
        }
    }
}
