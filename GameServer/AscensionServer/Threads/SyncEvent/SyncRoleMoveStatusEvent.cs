using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;

namespace AscensionServer.Threads
{
    public class SyncRoleMoveStatusEvent : SyncEvent
    {
        ThreadData<AscensionPeer> threadData;
        public override void Handler(object state)
        {
            if (threadData != null)
            {
                int peerCount = threadData.Collection.Count;
                var roleJson = Utility.Json.ToJson(threadData.Data.PeerCache.RoleMoveStatus);
                EventDataDict.Clear();
                EventDataDict.Add((byte)ParameterCode.RoleMoveStatus, roleJson);
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
