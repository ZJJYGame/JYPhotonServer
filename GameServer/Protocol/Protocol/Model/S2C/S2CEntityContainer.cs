using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CContainer : IDataContract
    {
        [Key(0)]
        public int ContainerId { get; set; }
        [Key(1)]
        public List<C2SPlayer> Players { get; set; }
        [Key(2)]
        public int MSPerTick { get; set; }
    }
}
