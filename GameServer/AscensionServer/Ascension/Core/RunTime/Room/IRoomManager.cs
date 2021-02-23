using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface IRoomManager:IModuleManager
    {
        void Allocate(ref RoomEntity room);
        bool TryGetValue(int key, out RoomEntity value);
        bool ContainsKey(int key);
        bool TryRemove(int key);
        bool TryRemove(int key, out RoomEntity value);
        bool TryAdd(int key, RoomEntity value);
        bool TryUpdate(int key, RoomEntity newValue, RoomEntity comparsionValue);
        void CloseAll();
    }
}


