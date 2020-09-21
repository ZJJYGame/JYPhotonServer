using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    /// <summary>
    /// 房间管理器，用于处理战斗相关
    /// </summary>
    public sealed class RoomManager:Module<RoomManager>
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
        ConcurrentDictionary<uint, RoomEntity> roomDict = new ConcurrentDictionary<uint, RoomEntity>();
        public bool AddRoom(RoomEntity roomCache)
        {
            return roomDict.TryAdd(roomCache.RoomId, roomCache);
        }
        public RoomEntity CreateRoom()
        {
            var room= GameManager.ReferencePoolManager.Spawn<RoomEntity>();
            uint roomId = CreateRoomId();
            if (roomDict.ContainsKey(roomId))
            {
                room.OnInit(roomId);
            }
            return room;
        }
        public bool RemoveRoom(uint roomId)
        {
            RoomEntity rc;
            return roomDict.TryRemove(roomId, out rc);
        }
        public RoomEntity  GetRoom(uint roomId)
        {
            RoomEntity rc;
            roomDict.TryGetValue(roomId, out rc);
            return rc;
        }
        public void CloseAll()
        {
            foreach (var  room in roomDict.Values)
            {
                GameManager.ReferencePoolManager.Despawn(room);
            }
            roomDict.Clear();
        }
        uint CreateRoomId()
        {
            uint id =Convert.ToUInt32( Utility.Algorithm.CreateRandomInt(_MinValue, _MaxValue));
            if (!roomDict.ContainsKey(id))
                return id;
            else
                return CreateRoomId();
        }

    }
}
