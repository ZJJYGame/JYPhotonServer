using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AscensionServer;
using Photon.SocketServer;
using Cosmos;
namespace AscensionServer.Threads
{
    public interface ISyncEvent:IBehaviour,IReference
    {
        void Handler(object state);
        Dictionary<byte, object> EventDataDict { get; }
        SendParameters SendParameter { get; set; }
        void SetData( IThreadData eventArgs);
        void AddFinishedHandler(Action handler);
        void ClearFinishedHandler();
    }
}
