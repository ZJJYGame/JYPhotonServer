using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class BattleManager:ModuleBase<BattleManager>
    {
        ConcurrentDictionary<int, RoomCache> roomDict = new ConcurrentDictionary<int, RoomCache>();

        public void StartBattle()
        {
            var rc= ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<RoomCache>();
        }
        /// <summary>
        /// 释放并回收战斗房间
        /// </summary>
        public void Release(int roomID)
        {
            RoomCache rc;
            roomDict.TryRemove(roomID, out rc);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(rc);
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
