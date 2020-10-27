using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class C2SEntityContainer: IDataContract
    {
        [Key(0)]
        public int EntityContainerId { get; set; }
        [Key(1)]
        public C2SPlayer Player { get; set; }
    }
}
