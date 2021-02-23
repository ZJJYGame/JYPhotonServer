using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public interface IServerTeamManager:IModuleManager
    {
        /// <summary>
        /// 玩家id 和队伍id 之间的映射
        /// </summary>
        Dictionary<int, int> PlayerIdToTeamIdDict { get; set; }
        /// <summary>
        /// 队伍id 和队伍信息模型映射
        /// </summary>
        Dictionary<int, TeamDTO> TeamTOModel { get; set; }
        /// <summary>
        /// 收集，释放解散的存在的队伍信息
        /// </summary>
        List<int> OldTeamList { get; set; }
        /// <summary>
        /// 判断是否是队长
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        bool IsLeader(int playerId);
        /// <summary>
        /// 服务器返回给客户端的参数
        /// </summary>
        Dictionary<byte, object> ServerToClientParams();




        #region 发送给客户端
        void ServerToClientInit(int roleId);
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roleId"></param>
        void ServerToClientCreate(int roleId);
        /// <summary>
        /// 申请加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        void ServerToClientApply(int teamLeader);
        /// <summary>
        /// 同意加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        void ServerToClientJoin(List<RoleDTO> roleDTOs);
        /// <summary>
        /// 拒绝加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        void ServerToClientRefused(int leaderId);
        /// <summary>
        ///转让队长
        /// </summary>
        /// <param name="roleDTOs"></param>
        void ServerToClientTransfer(List<RoleDTO> roleDTOs);
        /// <summary>
        /// 退出队伍 
        /// </summary>
        /// <param name="roleId"></param>
        void ServerToClientDissolveTeam(List<RoleDTO> roleDTOs);
        #endregion


        #region 组队的具体处理
        /// <summary>
        /// 创建一个队伍  
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <param name="levelLimint"></param>
        void CreateTeam(RoleDTO role, int[] levelLimint);
        /// <summary>
        /// 加入队伍申请
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <returns></returns>
        void ApplyJoinTeam(RoleDTO roleDTO, int teamId);

        /// <summary>
        /// 同意加入队伍  
        /// </summary>
        /// <param name="roleDTO"></param>
        void JoinTeam(RoleDTO roleDTO, int teamId);
        /// <summary>
        /// 拒绝队伍申请
        /// </summary>
        void RefusedApplyTeam(RoleDTO roleDTO, int teamId);

        /// <summary>
        /// 转让队长
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <param name="teamId"></param>
        void TransferTeam(RoleDTO roleDTO, int teamId);
        /// <summary>
        /// 离开队伍   踢人
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <param name="teamId"></param>
        void LevelTeam(RoleDTO roleDTO, int teamId);

        /// <summary>
        /// 退出队伍
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <param name="teamId"></param>
        void ExitTeam(RoleDTO roleDTO, int teamId);

        /// <summary>
        /// 调整站位
        /// </summary>
        void PositionTeam(RoleDTO roleDTO, int teamId);
        #endregion
    }
}


