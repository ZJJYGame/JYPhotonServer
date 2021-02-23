using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface ITeamManager:IModuleManager
    {
        TeamEntity CreateTeam(int createrId);
        bool LeaveMatchQueue(int roleId);
        /// <summary>
        ///加入随机匹配队列
        /// </summary>
        bool JoinMatchQueue(int roleId);
        /// <summary>
        /// 服务端会一直运行；
        /// 异步随机匹配组队
        /// </summary>
        /// <returns>异步运行</returns>
        Task MatchTeam();
        /// <summary>
        /// 加入指定的小队
        /// </summary>
        /// <param name="teamID">小队ID</param>
        /// <param name="roleId">peerID</param>
        /// <returns>是否加入成功</returns>
        bool JoinTeam(int teamID, int roleId);
    }
}


