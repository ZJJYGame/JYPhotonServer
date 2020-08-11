using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class BattleManager:ConcurrentSingleton<BattleManager>
    {
        ConcurrentDictionary<int, RoomCache> roomDict = new ConcurrentDictionary<int, RoomCache>();

        public void Add()
        {

        }
        public void Remove()
        {

        }
        public RoomCache Get(int roomID)
        {
            RoomCache rc;
            roomDict.TryGetValue(roomID, out rc);
            return rc;
        }
        public void CloseAll()
        {

        }
    }
}
