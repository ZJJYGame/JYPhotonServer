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
        List<RoleTransformDTO> roleTransformList = new List<RoleTransformDTO>();
        /// <summary>
        /// TODO 当前未使用瓦片算法进行分区域消息广播
        /// </summary>
        void BroadcastLoggedRolesPosition()
        {
            //var loggedList = AscensionServer.Instance.LoggedPeerCache.GetValuesList();
            var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            if (loggedList.Count <= 0)
                return;
            roleTransformList.Clear();
            try{EventData.Parameters.Clear();}catch {}
            EventDataDict.Clear();
            //roleTransformList.Capacity = loggedList.Count;
            for (int i = 0; i <loggedList.Count ; i++)
            {
                var roleDataTmp = Singleton<ReferencePoolManager>.Instance.Spawn<RoleTransformDTO>();
                roleDataTmp = loggedList[i].RoleTransform;
                roleTransformList.Add(roleDataTmp);
            }
            EventData.Code = (byte)EventCode.SyncRoleTransform;
            EventDataDict.Add((byte)ObjectParameterCode.RoleTransfromList, Utility.Json.ToJson(roleTransformList));
            //EventData = new EventData((byte)EventCode.SyncRoleTransform);
            EventData.Parameters = EventDataDict;
            for (int i = 0; i < loggedList.Count; i++)
            {
                loggedList[i].SendEvent(EventData, SendParameter);
            }
            Singleton<ReferencePoolManager>.Instance.Despawns(roleTransformList.ToArray());
            AscensionServer._Log.Info("BroadcastLoggedRolesPosition");
        }

    }
}
