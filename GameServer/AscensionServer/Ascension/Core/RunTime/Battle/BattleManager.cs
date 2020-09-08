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
    /// 战斗模块；
    /// 此模块用于转发客户端发送过来的消息到具体战斗容器中；
    /// 此模块非逻辑层，若处理逻辑，则在具体的战斗容器中处理；
    /// </summary>
    public sealed class BattleManager : Module<BattleManager>, IKeyValue<uint, RoomEntity>
    {
        /// <summary>
        /// 战斗房间的缓存
        /// </summary>
        ConcurrentDictionary<uint, RoomEntity> roomDict;
        IBattleMessageHelper battleMessageProvider;
        public void SetProvider(IBattleMessageHelper provider)
        {
            battleMessageProvider = provider;
        }
        public override void OnInitialization()
        {
            roomDict = new ConcurrentDictionary<uint, RoomEntity>();
            //NetworkEventCore.Instance.AddEventListener(NetworkEventParam.BATTLE_CMD, BroadCastBattleCmd);
            //NetworkEventCore.Instance.AddEventListener(NetworkEventParam.BATTLE_CMD, BroadCastBattleCmd);
        }
        void BroadCastBattleCmd(object data)
        {
            //if (data == null)
            //    return;
            //RoomCache rc;
            //var result = roomDict.TryGetValue(rbiCmd.RoomID, out rc);
            //if (result)
            //    rc.CacheInputCmdC2S(rbiCmd);
        }
        public RoomEntity GetBattleRoom(uint roomID)
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
        public bool TryGetValue(uint key, out RoomEntity value)
        {
            var result = roomDict.TryGetValue(key, out value);
            return result;
        }
        public bool ContainsKey(uint key)
        {
            return roomDict.ContainsKey(key);
        }
        public bool TryRemove(uint Key)
        {
            RoomEntity rc;
            var result = roomDict.TryRemove(Key, out rc);
            if (result)
                GameManager.ReferencePoolManager.Despawn(rc);
            return result;
        }
        public bool TryAdd(uint key, RoomEntity Value)
        {
            return roomDict.TryAdd(key, Value);
        }
        public bool TryUpdate(uint key, RoomEntity newValue, RoomEntity comparsionValue)
        {
            return roomDict.TryUpdate(key, newValue,comparsionValue);
        }
    }
}
