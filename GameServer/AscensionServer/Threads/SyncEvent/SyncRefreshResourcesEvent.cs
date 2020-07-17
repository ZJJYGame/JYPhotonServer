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
    public class SyncRefreshResourcesEvent : SyncEvent
    {
        public override void Handler(object state)
        {
            Thread.Sleep(AscensionConst.SyncResourceInterval);
            AdventureRefreshResources();
        }

        public override void OnInitialization()
        {
            base.OnInitialization();
            EventData.Parameters = EventDataDict;
        }

        /// <summary>
        /// 针对当前历练资源刷新的简易应对方法
        /// </summary>
        void AdventureRefreshResources()
        {
            HashSet<OccupiedUnitDTO> occupiedUnitDTOs = new HashSet<OccupiedUnitDTO>();

            occupiedUnitDTOs = AscensionServer.Instance.OccupiedUnitSetCache;
            var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var loggedCount = loggedList.Count;
            if (loggedCount <= 0)
                return;

            EventDataDict.Clear();
            EventData.Code = (byte)EventCode.RelieveOccupiedResourceUnit;
            EventDataDict.Add((byte)ParameterCode.RelieveUnit, Utility.Json.ToJson(occupiedUnitDTOs));
            for (int i = 0; i < loggedCount; i++)
            {
                loggedList[i].SendEvent(EventData, SendParameter);
            }

        }

    }
}
