/*
*Author : Don
*Since 	:2020-04-18
*Description  : 线程池
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AscensionServer.Threads
{
    public static class ThreadEvent
    {
        static HashSet<ISyncEvent> syncEventHash = new HashSet<ISyncEvent>();
        public static int SyncEventCount { get { return syncEventHash.Count; } }
        public static void AddSyncEvent(ISyncEvent syncEvent)
        {
            syncEventHash.Add(syncEvent);
        }
        public static void RemoveSyncEvent(ISyncEvent syncEvent)
        {
            syncEventHash.Remove(syncEvent);
        }
        public static void ExecuteEvent()
        {
            foreach (var threadEvent in syncEventHash)
            {
                ThreadPool.QueueUserWorkItem(threadEvent.Handler);
            }
        }
    }
}
