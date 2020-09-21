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
    /// 战斗模块；
    /// 此模块用于转发客户端发送过来的消息到具体战斗容器中；
    /// 此模块非逻辑层，若处理逻辑，则在具体的战斗容器中处理；
    /// </summary>
    public sealed class BattleManager : Module<BattleManager>
    {
        ConcurrentDictionary<int, RoomEntity> roomDict = new ConcurrentDictionary<int, RoomEntity>();

        public void StartBattle()
        {
            var rc = GameManager.ReferencePoolManager.Spawn<RoomEntity>();
        }
        /// <summary>
        /// 转发战斗消息
        /// </summary>
        /// <param name="rbiCmd">房间战斗消息命令</param>
        /// <returns>战斗系统是否能够成功转发消息</returns>
        public bool ForwardingBattleCmd(RoomBattleInputC2S rbiCmd)
        {
            if (rbiCmd == null)
                return false;
            RoomEntity rc;
            var result = roomDict.TryGetValue(rbiCmd.RoomID, out rc);
            //if (result)
            //    rc.CacheInputCmdC2S(rbiCmd);
            return result;
        }
        /// <summary>
        /// 释放并回收战斗房间
        /// </summary>
        public void ReleaseBattleRoom(int roomID)
        {
            RoomEntity rc;
            roomDict.TryRemove(roomID, out rc);
            GameManager.ReferencePoolManager.Despawn(rc);
        }
        public RoomEntity GetBattleRoom(int roomID)
        {
            RoomEntity rc;
            roomDict.TryGetValue(roomID, out rc);
            return rc;
        }
        public void CloseAll()
        {
            foreach (var room in roomDict.Values)
            {
                room.Clear();
            }
            roomDict.Clear();
        }
    }
}
