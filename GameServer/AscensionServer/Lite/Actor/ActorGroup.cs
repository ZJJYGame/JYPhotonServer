using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Lite

{
    public class ActorGroup : ActorCollection
    {
        public ActorGroup(byte id)
        {
            GroupId = id;
        }

        public byte GroupId { get; private set; }
    }
}


