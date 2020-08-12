using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 房间管理器，用于处理战斗相关
    /// </summary>
    public class RoomManager:ConcurrentSingleton<RoomManager>
    {
        ConcurrentDictionary<int, RoomCache> roomDict = new ConcurrentDictionary<int, RoomCache>();
        public bool AddRoom(RoomCache roomCache)
        {
            return roomDict.TryAdd(roomCache.RoomID, roomCache);
        }
        public bool RemoveRoom(int roomID)
        {
            RoomCache rc;
            return roomDict.TryRemove(roomID, out rc);
        }
        public RoomCache GetRoom(int roomID)
        {
            RoomCache rc;
            roomDict.TryGetValue(roomID, out rc);
            return rc;
        }
        public void CloseAll()
        {
            foreach (var  room in roomDict.Values)
            {
                room.OnTermination();
            }
            roomDict.Clear();
        }
    }
}
