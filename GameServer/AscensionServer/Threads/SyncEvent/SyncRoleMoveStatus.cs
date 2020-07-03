using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AscensionServer.Threads
{
    public class SyncRoleMoveStatus : SyncEvent
    {
        List<RoleMoveStatusDTO> syncRoleMoveStatuses = new List<RoleMoveStatusDTO>();
        public override void Handler(object state)
        {
            while (true)
            {
                Thread.Sleep(AscensionConst.SyncRoleMoveStasus);
                BroadcastLoggedRolesMoveStatus();
            }
        }
        void BroadcastLoggedRolesMoveStatus()
        {
            var statusList =   AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var statusCount = statusList.Count;
            if (statusCount <= 0) return;

            syncRoleMoveStatuses.Clear();
            EventData.Parameters = EventDataDict;
            for (int i = 0; i < statusCount; i++)
            {
                var roleMoveStatus = Singleton<ReferencePoolManager>.Instance.Spawn<RoleMoveStatusDTO>();
                roleMoveStatus = statusList[i].RoleMoveStatus;
                syncRoleMoveStatuses.Add(roleMoveStatus);
            }

            EventData.Code = (byte)EventCode.SyncRoleMoveStatus;
            EventDataDict.Clear();
            EventDataDict.Add((byte)ParameterCode.RoleMoveStatus, Utility.Json.ToJson(syncRoleMoveStatuses));
            for (int i = 0; i < statusList.Count; i++)
            {
                statusList[i].SendEvent(EventData, SendParameter);
            }
            Singleton<ReferencePoolManager>.Instance.Despawns(syncRoleMoveStatuses.ToArray());
        }
    }
}
