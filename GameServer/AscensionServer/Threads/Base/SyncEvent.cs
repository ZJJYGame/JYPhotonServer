using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Threads
{
    public abstract class SyncEvent : ISyncEvent
    {
        public Dictionary<byte, object> EventDataDict { get; protected set; }
        public SendParameters SendParameter { get; set; }
        public abstract void Handler(object state);
        public virtual void OnInitialization() {
            EventDataDict = new Dictionary<byte, object>();
            SendParameter = new SendParameters();
        }
        public  virtual void OnTermination() { }
    }
}
