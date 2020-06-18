﻿using Cosmos;
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
            EventData = new EventData();
        }
        protected EventData EventData { get; set; }
        public  virtual void OnTermination()
        {
            EventDataDict.Clear();
            EventData.Parameters.Clear();
            ClearFinishedHandler();
        }
        /// <summary>
        /// 空的虚函数
        /// </summary>
        /// <param name="eventArgs"></param>
        public virtual void SetData(IThreadData eventArgs) { }
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
