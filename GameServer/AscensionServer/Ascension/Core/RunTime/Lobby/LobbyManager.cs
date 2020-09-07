using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 玩家的大厅管理器
    /// </summary>
    public sealed class LobbyManager : Module<LobbyManager>, ISimpleKeyValue<long, PeerEntity>
    {
        Dictionary<long, PeerEntity> peerDict;
        public override void OnInitialization()
        {
            peerDict = new Dictionary<long, PeerEntity>();
        }
        public bool TryAdd(long sessionId, PeerEntity peerEntity)
        {
            var result = peerDict.ContainsKey(sessionId);
            if (!result)
                peerDict.Add(sessionId, peerEntity);
            return result;
        }
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="sessionId">peerID</param>
        /// <returns>是否离开成功</returns>
        public bool TryRemove(long sessionId)
        {
            return peerDict.Remove(sessionId);
        }
        public bool TryGetValue(long sessionId, out PeerEntity peer)
        {
            peer = default;
            if (!ContainsKey(sessionId))
                return false;
            return GameManager.External.GetModule<PeerManager>().TryGetValue(sessionId, out peer);
        }
        /// <summary>
        /// 是否在大厅中
        /// </summary>
        /// <param name="sessionId">id</param>
        /// <returns>是否存在</returns>
        public bool ContainsKey(long sessionId)
        {
            return peerDict.ContainsKey(sessionId);
        }
    }
}
