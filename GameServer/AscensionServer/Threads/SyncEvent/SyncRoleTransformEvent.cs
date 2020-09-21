using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer;
using AscensionProtocol.DTO;
using AscensionProtocol;
//using ExitGames.Client.Photon;
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
                Thread.Sleep(ApplicationBuilder.SyncLoggedRolesPositionInterval);
                BroadcastTransform();
            }
        }
        public override void OnInitialization()
        {
            base.OnInitialization();
            //EventData.Parameters = EventDataDict;
            EventData.Code = (byte)EventCode.SyncRoleTransform;
        }
        /// <summary>
        /// TODO 当前未使用瓦片算法进行分区域消息广播
        /// </summary>
        void BroadcastTransform()
        {
            //AscensionServer._Log.Info("BroadcastLoggedRolesPosition 同步位置");
            HashSet<RoleTransformQueueDTO> roleTransformSet = new HashSet<RoleTransformQueueDTO>();
            //var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            //var loggedCount = loggedList.Count;
            //if (loggedCount <= 0)
            //    return;
            //for (int i = 0; i < loggedCount; i++)
            //{
            //    if (!loggedList[i].IsSendedTransform)
            //    {
            //        roleTransformSet.Add(loggedList[i].PeerCache.RoleTransformQueue);
            //        loggedList[i].IsSendedTransform = true;
            //    }
            //}
            //EventDataDict.Clear();
            //ConcurrentEventDataDict.TryAdd((byte)ParameterCode.RoleTransformQueueSet, Utility.Json.ToJson(roleTransformSet));
            var data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.RoleTransformQueueSet, Utility.Json.ToJson(roleTransformSet));
            EventData.Parameters = data;
            //for (int i = 0; i < loggedList.Count; i++)
            //{
            //    loggedList[i].SendEvent(EventData, SendParameter);
            //}
            //foreach (var p in loggedList)
            //{
            //    p.SendEvent(EventData, SendParameter);
            //}
        }
    }
}
