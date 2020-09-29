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
    public sealed class RoomManager : Module<RoomManager>, IKeyValue<int, RoomEntity>
    {
        readonly int _IntervalSec = 30;
        long latestTime;
        int roomId= 10000;
        ConcurrentDictionary<int, RoomEntity> roomDict 
            = new ConcurrentDictionary<int, RoomEntity>();
        public void Allocate(ref RoomEntity room)
        {
            TryAdd(room.RoomId, room);
        }
        /// <summary>
        /// 周期轮询释放无用的房间对象；
        /// </summary>
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var nowSec = Utility.Time.SecondNow();
            if (latestTime <= nowSec)
            {
                latestTime = nowSec;
                foreach (var room in roomDict.Values)
                {
                    if (!room.Available)
                    {
                        RoomEntity roomEntity;
                        roomDict.TryRemove(room.RoomId, out roomEntity);
                    }
                }
            }
        }
        public bool TryGetValue(int key, out RoomEntity value)
        {
            return roomDict.TryGetValue(key, out value);
        }
        public bool ContainsKey(int key)
        {
            return roomDict.ContainsKey(key);
        }
        public bool TryRemove(int key)
        {
            RoomEntity re;
            return roomDict.TryRemove(key,out re);
        }
        public bool TryRemove(int key, out RoomEntity value)
        {
            return roomDict.TryRemove(key, out value);
        }
        public bool TryAdd(int key, RoomEntity value)
        {
            return roomDict.TryAdd(key, value);
        }
        public bool TryUpdate(int key, RoomEntity newValue, RoomEntity comparsionValue)
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
        public override void OnTermination()
        {
            CloseAll();
        }
    }
}
