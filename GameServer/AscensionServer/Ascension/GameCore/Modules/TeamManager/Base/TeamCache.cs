using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class TeamCache : IReference
    {
        /// <summary>
        /// 队长ID
        /// </summary>
        public int CaptainID{ get; set; }
        /// <summary>
        /// 系统分配的队伍ID
        /// </summary>
        public int TeamID { get; set; }
        /// <summary>
        /// 是否队伍满员
        /// </summary>
        public bool IsFull { get { return peerDict.Count >=_TeamCapacity; } }
        readonly short _TeamCapacity = 5;
        ConcurrentDictionary<int, AscensionPeer> peerDict = new ConcurrentDictionary<int, AscensionPeer>();
        AscensionPeer captain;
        /// <summary>
        /// 初始化队伍
        /// </summary>
        /// <param name="createrID">创建者的ID</param>
        public void InitTeam(int createrID,int teamID)
        {
            TeamID = teamID;
            CaptainID = createrID;
            captain = RoleManager.Instance.GetPeer(createrID);
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="peerID">peerID</param>
        /// <param name="peer">peer对象</param>
        /// <returns>是否加入成功</returns>
        public bool JoinTeam(int peerID)
        {
            var peer = RoleManager.Instance.GetPeer(peerID);
            return peerDict.TryAdd(peerID,peer);
        }
        /// <summary>
        /// 主动离队
        /// </summary>
        /// <param name="peerID"></param>
        /// <returns>是否离队成功</returns>
        public bool LeaveTeam(int peerID)
        {
            AscensionPeer peer;
            return peerDict.TryRemove(peerID, out peer);
        }
        /// <summary>
        /// 被请离队伍；
        /// 值一队长有权限
        /// </summary>
        /// <param name="peerID"></param>
        public bool KickOutOfTeam(int cmdInputterID,int peerID)
        {
            if (cmdInputterID != CaptainID)
                return false;
            return LeaveTeam(peerID);
        }
        /// <summary>
        /// 解散队伍
        /// </summary>
        public void DissolveTeam()
        {
            foreach (var peer in peerDict.Values)
            {
                //广播解散队伍的消息
            }
            Clear();
        }
        /// <summary>
        /// 提升为队长；
        /// 只有前队长才进行的命令;
        /// </summary>
        /// <param name=""></param>
        public void PreferredToCaptain(int cmdInputterID,int peerID)
        {
            if (cmdInputterID != CaptainID)
                return;
            CaptainID = peerID;
            // 广播成为队长事件
        }
        public void Clear()
        {
            peerDict.Clear();
            CaptainID = -1;
            captain = null;
        }
    }
}
