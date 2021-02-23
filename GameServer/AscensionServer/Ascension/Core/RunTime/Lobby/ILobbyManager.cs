using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface ILobbyManager:IModuleManager
    {
        bool TryAdd(int sessionId);
        /// <summary>
        /// 离开大厅；
        /// 这个离开可能玩家进行了进入探索界面，或者副本
        /// </summary>
        /// <param name="sessionId">peerID</param>
        /// <returns>是否离开成功</returns>
        bool TryRemove(int sessionId);
        /// <summary>
        /// 是否在大厅中
        /// </summary>
        /// <param name="sessionId">id</param>
        /// <returns>是否存在</returns>
        bool Contains(int sessionId);
        /// <summary>
        /// 在大厅中通过ID查找Peer
        /// </summary>
        /// <param name="sessionId">id</param>
        /// <returns>查找到的对象</returns>
        bool TryGetValue(int sessionId, out IPeerEntity peer);
    }
}


