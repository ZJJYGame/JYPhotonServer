using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Cosmos;
namespace AscensionServer
{
    public class TeamEntity: IReference
    {
        /// <summary>
        /// 队长ID
        /// </summary>
        public int CaptainId{ get; set; }
        /// <summary>
        /// 系统分配的队伍ID
        /// </summary>
        public int TeamID { get; set; }
        /// <summary>
        /// 是否队伍满员
        /// </summary>
        public bool IsFull { get { return peerDict.Count >=_TeamCapacity; } }
        readonly ushort _TeamCapacity = 5;
        ConcurrentDictionary<int, IRemoteRole> peerDict = new ConcurrentDictionary<int, IRemoteRole>();
        IPeerAgent captain;
        /// <summary>
        /// 初始化队伍
        /// </summary>
        /// <param name="createrId">创建者的ID</param>
        public void Oninit(int createrId,int teamID)
        {
            TeamID = teamID;
            CaptainId = createrId;
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="roleId">peerID</param>
        /// <returns>是否加入成功</returns>
        public bool JoinTeam(int roleId)
        {
            return peerDict.TryAdd(roleId,null);
        }
        /// <summary>
        /// 主动离队
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>是否离队成功</returns>
        public bool LeaveTeam(int roleId)
        {
            IRemoteRole peer;
            return peerDict.TryRemove(roleId, out peer);
        }
        /// <summary>
        /// 被请离队伍；
        /// 值一队长有权限
        /// </summary>
        public bool KickOutOfTeam(int cmdInputterID,int  roleId)
        {
            if (cmdInputterID != CaptainId)
                return false;
            return LeaveTeam(roleId);
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
        public void PreferredToCaptain(uint cmdInputterID,int roleId)
        {
            if (cmdInputterID != CaptainId)
                return;
            CaptainId = roleId;
            // 广播成为队长事件
        }
        public void Clear()
        {
            peerDict.Clear();
            CaptainId = 0;
            captain = null;
        }
    }
}
