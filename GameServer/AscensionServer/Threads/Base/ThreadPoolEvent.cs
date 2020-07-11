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
    /// <summary>
    /// 线程池事件，用于处理需要开启新线程进行处理的事件；
    /// 例如同步玩家位置信息
    /// </summary>
    public static class ThreadPoolEvent
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
