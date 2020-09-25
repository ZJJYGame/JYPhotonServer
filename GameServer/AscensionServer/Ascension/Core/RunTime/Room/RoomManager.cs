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
    public sealed class RoomManager : Module<RoomManager>, IKeyValue<uint, RoomEntity>
    {
        /// <summary>
        /// 房间ID长度
        /// </summary>
        uint roomId= 10000;
        ConcurrentDictionary<uint, RoomEntity> roomDict 
            = new ConcurrentDictionary<uint, RoomEntity>();
        //public RoomEntity CreateRoom()
        //{
        //    var room = GameManager.ReferencePoolManager.Spawn<RoomEntity>();
        //    uint roomId;
        //    if (roomDict.ContainsKey(roomId))
        //    {
        //        room.OnInit(roomId);
        //    }
        //    return room;
        //}
        /// <summary>
        /// 为roomEntity分配房间号；
        /// </summary>
        /// <param name="room">传入的房间对象</param>
        public void Allocate(ref RoomEntity room)
        {
            TryAdd(room.RoomId, room);
        }
        public bool TryGetValue(uint key, out RoomEntity value)
        {
            return roomDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(uint key)
        {
            return roomDict.ContainsKey(key);
        }
        public bool TryRemove(uint key)
        {
            RoomEntity re;
            return roomDict.TryRemove(key,out re);
        }
        public bool TryRemove(uint key, out RoomEntity value)
        {
            return roomDict.TryRemove(key, out value);
        }
        public bool TryAdd(uint key, RoomEntity value)
        {
            return roomDict.TryAdd(key, value);
        }
        public bool TryUpdate(uint key, RoomEntity newValue, RoomEntity comparsionValue)
        {
            return roomDict.TryUpdate(key, newValue,comparsionValue);
        }
        public void CloseAll()
        {
            foreach (var room in roomDict.Values)
            {
                GameManager.ReferencePoolManager.Despawn(room);
            }
            roomDict.Clear();
        }
    }
}
