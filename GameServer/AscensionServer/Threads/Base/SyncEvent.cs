using Cosmos;
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
        protected IThreadEventData threadEventData;
        public virtual void Handler(object state)
        {
            if (threadEventData != null)
            {
                EventData.Parameters = threadEventData.Parameters;
                EventData.Code = threadEventData.EventCode;
                foreach (var peer in threadEventData.PeerCollection)
                {
                    peer.SendEvent(EventData, SendParameter);
                }
            }
            finishedHandler?.Invoke();
        }
        public virtual void OnInitialization() {
            EventDataDict = new Dictionary<byte, object>();
            SendParameter = new SendParameters();
            EventData = new EventData();
        }
        protected EventData EventData { get; set; }
        public  virtual void OnTermination()
        {
            EventDataDict.Clear();
            EventData.Parameters.Clear();
            ClearFinishedHandler();
            threadEventData.Clear();
        }
        public  void SetEventData(IThreadEventData threadEventData)
        { this.threadEventData = threadEventData; }
        public void AddFinishedHandler(Action handler)
        {
            finishedHandler += handler;
        }
       protected Action finishedHandler;
        public void ClearFinishedHandler()
        {
            finishedHandler = null;
        }
        public void Clear()
        {
            OnTermination();
        }
    }
}
