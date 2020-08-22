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
using AscensionRegion;
using AscensionData;

namespace AscensionServer.Threads
{
    public class SyncRefreshResourcesEvent : SyncEvent
    {
        public override void Handler(object state)
        {
            while (true)
            {
                Thread.Sleep(AscensionConst.SyncResourceInterval);
                //AscensionServer._Log.Info("刷新通知 SyncRefreshResourcesEvent");
                //if (AscensionServer.Instance.OccupiedUnitSetCache.Count <= 0)
                //    return;
                AdventureRefreshResources();
            }
        }

        public override void OnInitialization()
        {
            base.OnInitialization();
            EventData.Code = (byte)EventCode.RelieveOccupiedResourceUnit;
        }

        /// <summary>
        /// 针对当前历练资源刷新的简易应对方法
        /// </summary>
        void AdventureRefreshResources()
        {
            //AscensionServer._Log.Info("刷新通知");
            HashSet<OccupiedUnitDTO> occupiedUnitDTOs = AscensionServer.Instance.OccupiedUnitSetCache;
            var loggedList = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            var loggedCount = loggedList.Count;
            if (loggedCount <= 0)
                return;
            Vector2 border = new Vector2(54000, 39000);
            foreach (var occupiedUnitObj in occupiedUnitDTOs)
            {
                ResourceUnitSetDTO currentDictObj = null;
                if (AscensionServer.Instance.ResUnitSetDict.TryGetValue(occupiedUnitObj.GlobalID, out currentDictObj))
                {

                    ResourceUnitDTO resourceUnitDTO = null;
                    if (currentDictObj.ResUnitDict.TryGetValue(occupiedUnitObj.ResID, out resourceUnitDTO))
                    {
                        resourceUnitDTO.Occupied = false;
                        var randonVector = ConcurrentSingleton<ResourceCreator>.Instance.GetRandomVector2(Vector2.Zero, border);
                        var randonRotate = ConcurrentSingleton<ResourceCreator>.Instance.GetRandomVector3(Vector3.Zero, new Vector3(0, 360000, 0));
                        resourceUnitDTO.Position = new TransformDTO() { PositionX = randonVector.X, PositionY = 0, PositionZ = randonVector.Y, RotationX = 0, RotationY = randonRotate.Y, RotationZ = 0 };
                        occupiedUnitObj.Position = resourceUnitDTO.Position;
                    }
                }
            }
            var data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.RelieveUnit, Utility.Json.ToJson(occupiedUnitDTOs));
            EventData.Parameters = data;
            AscensionServer.Instance.OccupiedUnitSetCache.Clear();
            foreach (var p in loggedList)
            {
                p.SendEvent(EventData, SendParameter);
            }
        }
    }
}
