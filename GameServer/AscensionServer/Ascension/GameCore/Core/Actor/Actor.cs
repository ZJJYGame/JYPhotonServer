using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class Actor : IActor
    {
        public byte ActorType { get; private set; }
        public AscensionPeer Peer { get; set; }
        public Actor(AscensionPeer peer)
        {
            this.Peer = peer;
        }
    }
}
