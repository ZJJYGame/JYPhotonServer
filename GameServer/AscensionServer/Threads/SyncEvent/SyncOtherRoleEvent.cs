using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer.Threads
{
    /// <summary>
    /// 这个线程事件，可以满足离开或者进入探索场景的人物生成的需求
    /// </summary>
    public class SyncOtherRoleEvent : SyncEvent
    {
        ThreadData<AscensionPeer> threadData;
        public override void Handler(object state)
        {
            if (threadData!= null)
            {
                int peerCount = threadData.Collection.Count;
                var roleJson = Utility.Json.ToJson(threadData.Data.PeerCache.Role);
                var roleMoveStatusJson = Utility.Json.ToJson(threadData.Data.RoleMoveStatus);
                var roleTransformJson = Utility.Json.ToJson(threadData.Data.RoleTransform);
                EventDataDict.Clear();
                EventDataDict.Add((byte)ParameterCode.Role, roleJson);
                EventDataDict.Add((byte)ParameterCode.RoleMoveStatus, roleMoveStatusJson);
                EventDataDict.Add((byte)ParameterCode.RoleTransfrom, roleTransformJson);
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
            threadData= eventArgs as ThreadData<AscensionPeer>;
        }
    }
}
