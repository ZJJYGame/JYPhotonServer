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
        static HashSet<ISyncEvent> syncEventHashSet = new HashSet<ISyncEvent>();
        public static int SyncEventCount { get { return syncEventHashSet.Count; } }
        public static void AddSyncEvent(ISyncEvent syncEvent)
        {
            syncEvent.OnInitialization();
            syncEventHashSet.Add(syncEvent);
        }
        public static void RemoveSyncEvent(ISyncEvent syncEvent)
        {
            syncEvent.OnTermination();
            syncEventHashSet.Remove(syncEvent);
        }
        public static void ExecuteEvent()
        {
            foreach (var threadEvent in syncEventHashSet)
            {
                ThreadPool.QueueUserWorkItem(threadEvent.Handler);
            }
        }
    }
}
