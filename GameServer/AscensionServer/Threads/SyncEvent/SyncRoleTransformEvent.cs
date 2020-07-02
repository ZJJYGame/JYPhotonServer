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
            List<RoleTransformDTO> roleTransformSet = new List<RoleTransformDTO>();
            //var loggedList = AscensionServer.Instance.LoggedPeerCache.GetValuesList();
            var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var loggedCount = loggedList.Count;
            if (loggedCount <= 0)
                return;
            //roleTransformSet.Clear();
            //try{EventData.Parameters.Clear();}catch {}
            EventData.Parameters = EventDataDict;
            for (int i = 0; i < loggedCount; i++)
            {
                var roleDataTmp = Singleton<ReferencePoolManager>.Instance.Spawn<RoleTransformDTO>();
                roleDataTmp = loggedList[i].RoleTransform;
                roleTransformSet.Add(roleDataTmp);
            }
            EventData.Code = (byte)EventCode.SyncRoleTransform;
            EventDataDict.Clear();
            EventDataDict.Add((byte)ParameterCode.RoleTransfromSet, Utility.Json.ToJson(roleTransformSet));
            for (int i = 0; i < loggedList.Count; i++)
            {
                loggedList[i].SendEvent(EventData, SendParameter);
            }
            Singleton<ReferencePoolManager>.Instance.Despawns(roleTransformSet.ToArray());
            //AscensionServer._Log.Info("BroadcastLoggedRolesPosition");
        }

    }
}
