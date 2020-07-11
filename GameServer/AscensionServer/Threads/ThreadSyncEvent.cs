using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Threads
{
    /// <summary>
    /// 线程的同步事件；
    /// 若无特殊线程，例如同步玩家位置信息这类特殊线程事件，其余都统一使用此线程同步事件
    /// </summary>
    public class ThreadSyncEvent:SyncEvent
    {
    }
}
