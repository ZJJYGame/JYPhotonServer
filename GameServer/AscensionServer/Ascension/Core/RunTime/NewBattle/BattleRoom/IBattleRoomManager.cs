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
        /// <summary>
        /// 创建战斗房间
        /// </summary>
        /// <param name="roleId">玩家id</param>
        /// <param name="enemyGlobalIds">敌人公共id集合</param>
        BattleRoomEntityInfo CreateRoom(int roleId, List<int> enemyGlobalIds);
    }
}
