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
        #region Properties
        public Dictionary<byte, object> EventDataDict { get; protected set; }
        public SendParameters SendParameter { get; set; }
        protected IThreadEventData threadEventData;
        protected Action finishedHandler;
        protected EventData EventData { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 被线程调用的委托
        /// </summary>
        /// <param name="state">委托参数</param>
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
        public virtual void OnInitialization()
        {
            EventDataDict = new Dictionary<byte, object>();
            SendParameter = new SendParameters();
            EventData = new EventData();
        }
        public virtual void OnTermination()
        {
            EventDataDict.Clear();
            EventData.Parameters.Clear();
            ClearFinishedHandler();
            threadEventData.Clear();
        }
        public void SetEventData(IThreadEventData threadEventData)
        {
            this.threadEventData = threadEventData;
        }
        /// <summary>
        /// 添加线程执行结束后的委托
        /// </summary>
        /// <param name="handler"></param>
        public void AddFinishedHandler(Action handler)
        {
            finishedHandler += handler;
        }
        public void ClearFinishedHandler()
        {
            finishedHandler = null;
        }
        public void Clear()
        {
            OnTermination();
        }
        #endregion
    }
}
