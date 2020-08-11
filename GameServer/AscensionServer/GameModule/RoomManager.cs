using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class RoomManager:ConcurrentSingleton<RoomManager>
    {
        ConcurrentDictionary<int, RoomCache> roomDict = new ConcurrentDictionary<int, RoomCache>();
        public bool Add(RoomCache roomCache)
        {
            return roomDict.TryAdd(roomCache.RoomID, roomCache);
        }
        public bool Remove(int roomID)
        {
            RoomCache rc;
            return roomDict.TryRemove(roomID, out rc);
        }
        public RoomCache Get(int roomID)
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
