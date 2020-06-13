using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AscensionServer;
using Photon.SocketServer;

namespace AscensionServer.Threads
{
    public interface ISyncEvent:IBehaviour
    {
        void Handler(object state);
        Dictionary<byte, object> EventDataDict { get; }
        SendParameters SendParameter { get; set; }
    }
}
