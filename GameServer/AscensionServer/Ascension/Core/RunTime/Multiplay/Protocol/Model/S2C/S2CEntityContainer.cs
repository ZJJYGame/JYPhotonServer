using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using Cosmos;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public class S2CContainer 
    {
        public int ContainerId { get; set; }
        public List<SessionRoleIdPair> Players { get; set; }
        public int MSPerTick { get; set; }
    }
}
