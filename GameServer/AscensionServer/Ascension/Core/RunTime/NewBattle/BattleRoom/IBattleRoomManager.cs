using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public interface IBattleRoomManager:IModuleManager
    {
        float PrepareWaitTime { get; }
        float RoundTIme { get; }
        float PerformWaitTime { get; }
        event Action TimeAction;
        BattleRoomEntity GetBattleRoomEntity(int roomID);
    }
}
