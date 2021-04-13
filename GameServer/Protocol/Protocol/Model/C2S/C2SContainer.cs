﻿using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Serializable]
    [MessagePackObject(true)]
    public class C2SContainer: IDataContract
    {
        public FixContainer Container { get; set; }
        public SessionRoleIdPair Player { get; set; }
    }
}
