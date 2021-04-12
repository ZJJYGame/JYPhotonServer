using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CContainer : IDataContract
    {
        [Key(0)]
        public int ContainerId { get; set; }
        [Key(1)]
        public List<SessionRoleIdPair> Players { get; set; }
        [Key(2)]
        public int MSPerTick { get; set; }
    }
}
