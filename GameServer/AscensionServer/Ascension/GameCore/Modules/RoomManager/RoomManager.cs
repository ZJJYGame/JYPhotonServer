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
    public class RoomManager:ModuleBase<RoomManager>
    {
        /// <summary>
        /// 房间ID长度
        /// </summary>
        readonly int _IDLenth = 7;
        /// <summary>
        /// 生成房间时最小取值范围
        /// </summary>
        readonly int _MinValue = 1000000;
        /// <summary>
        /// 成功房间时最大取值范围
        /// </summary>
        readonly int _MaxValue = 99999999;
        ConcurrentDictionary<int, RoomCache> roomDict = new ConcurrentDictionary<int, RoomCache>();
        public bool AddRoom(RoomCache roomCache)
        {
            return roomDict.TryAdd(roomCache.RoomID, roomCache);
        }
        public RoomCache CreateRoom()
        {
            var room= ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<RoomCache>();
            int roomID = CreateRoomID();
            if (roomDict.ContainsKey(roomID))
            {
                room.InitRoom(roomID);
            }
            return room;
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
                ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(room);
            }
            roomDict.Clear();
        }
        int CreateRoomID()
        {
            int id = Utility.Algorithm.CreateRandomInt(_MinValue, _MaxValue);
            if (!roomDict.ContainsKey(id))
                return id;
            else
                return CreateRoomID();
        }

    }
}
