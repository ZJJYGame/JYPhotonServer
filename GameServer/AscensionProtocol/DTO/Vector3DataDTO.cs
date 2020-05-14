﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
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
