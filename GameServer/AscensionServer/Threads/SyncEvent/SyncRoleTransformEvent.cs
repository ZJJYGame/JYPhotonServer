using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer;
using AscensionProtocol.DTO;
using AscensionProtocol;
using ExitGames.Client.Photon;
using System.Timers;
using Photon.SocketServer;
using EventData = Photon.SocketServer.EventData;
using System.Threading;
using Cosmos;
namespace AscensionServer.Threads
{
    public class SyncRoleTransformEvent : SyncEvent
    {
        public override  void  Handler(object state)
        {
            while (true)
            {
                Thread.Sleep(AscensionConst.SyncLoggedRolesPositionInterval);
                BroadcastLoggedRolesPosition();
            }
        }
        /// <summary>
        /// TODO 当前未使用瓦片算法进行分区域消息广播
        /// </summary>
        void BroadcastLoggedRolesPosition()
        {
            HashSet<RoleTransformSetDTO> roleTransformSet = new HashSet<RoleTransformSetDTO>();
            var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var loggedCount = loggedList.Count;
            if (loggedCount <= 0)
                return;
            EventData.Parameters = EventDataDict;
            for (int i = 0; i < loggedCount; i++)
            {
                if (!loggedList[i].IsSendedTransform)
                {
                    roleTransformSet.Add(loggedList[i].RoleTransformSetDTO);
                    loggedList[i].IsSendedTransform = true;
                }
            }
            EventData.Code = (byte)EventCode.SyncRoleTransform;
            EventDataDict.Clear();
            EventDataDict.Add((byte)ParameterCode.SingleRoleTransformSet, Utility.Json.ToJson(roleTransformSet));
            for (int i = 0; i < loggedList.Count; i++)
            {
                loggedList[i].SendEvent(EventData, SendParameter);
            }
        }
    }
}
