using AscensionProtocol;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Threads
{
    /// <summary>
    /// 同步占用的资源给所有客户端事件s
    /// </summary>
    public class SyncOccupiedUnitEvent : SyncEvent
    {
        ThreadData<AscensionPeer> threadData;
        public override void Handler(object state)
        {
            //广播被占用的资源：全局ID，分配的资源ID
            if (threadData != null)
            {
                int peerCount = threadData.Collection.Count;
                var occupiedUnitJson = Utility.Json.ToJson(AscensionServer.Instance.OccupiedUnitSetCache);
                EventDataDict.Clear();
                EventDataDict.Add((byte)ParameterCode.OccupiedUnit, occupiedUnitJson);
                EventData.Parameters = EventDataDict;
                EventData.Code = threadData.EventCode;
                foreach (var peer in threadData.Collection)
                {
                    peer.SendEvent(EventData, SendParameter);
                }
            }
            //
            finishedHandler?.Invoke();
        }
        public override void SetData(IThreadData eventArgs)
        {
            threadData = eventArgs as ThreadData<AscensionPeer>;
        }
    }
}
