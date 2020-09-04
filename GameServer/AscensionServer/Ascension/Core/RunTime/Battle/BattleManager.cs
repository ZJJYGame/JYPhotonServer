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
    public sealed class BattleManager : Module<BattleManager>, IKeyValue<uint, RoomCache>
    {
        /// <summary>
        /// 战斗房间的缓存
        /// </summary>
        ConcurrentDictionary<uint, RoomCache> roomDict;
        IBattleMessageHelper battleMessageProvider;
        public void SetProvider(IBattleMessageHelper provider)
        {
            battleMessageProvider = provider;
        }
        public override void OnInitialization()
        {
            roomDict = new ConcurrentDictionary<uint, RoomCache>();
            NetworkEventCore.Instance.AddEventListener(NetworkEventParam.BATTLE_CMD, BroadCastBattleCmd);
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
        public RoomCache GetBattleRoom(uint roomID)
        {
            RoomCache rc;
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
        public bool TryGetValue(uint key, out RoomCache value)
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
            RoomCache rc;
            var result = roomDict.TryRemove(Key, out rc);
            if (result)
                GameManager.ReferencePoolManager.Despawn(rc);
            return result;
        }
        public bool TryAdd(uint key, RoomCache Value)
        {
            return roomDict.TryAdd(key, Value);
        }
        public bool TryUpdate(uint key, RoomCache newValue, RoomCache comparsionValue)
        {
            return roomDict.TryUpdate(key, newValue,comparsionValue);
        }
    }
}
