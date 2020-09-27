﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Threads;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;
using AscensionServer.Model;
using AscensionData;
using AscensionRegion;
using System.Threading;
namespace AscensionServer
{
    /// <summary>
    /// 待完善
    /// 未统一的服务器端组队功能；
    /// </summary>
    [CustomeModule]
    public class ServerTeamManager:Module<ServerTeamManager>
    {
        /// <summary>
        /// 玩家id 和队伍id 之间的映射
        /// </summary>
        public Dictionary<int, int> _playerIdToTeamIdDict = new Dictionary<int, int>();
        /// <summary>
        /// 队伍id 和队伍信息模型映射
        /// </summary>
        public Dictionary<int, TeamDTO> _teamTOModel = new Dictionary<int, TeamDTO>();
        /// <summary>
        /// 收集，释放解散的存在的队伍信息
        /// </summary>
        public List<int> _oldTeamList = new List<int>();

        int teamid = 1000;

        /// <summary>
        /// 创建一个队伍  
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <param name="levelLimint"></param>
        public void CreateTeam(RoleDTO role, int[] levelLimint)
        {
            int playerId = role.RoleID;
            //Utility.Debug.LogInfo(playerId + "<>");
            //Utility.Debug.LogInfo(IsLeader(playerId) + "<>");
            if (IsLeader(playerId)) return;

            List<RoleDTO> roleDTOs = new List<RoleDTO>();
            roleDTOs.Add(role);

            TeamDTO teamDto;
            if (_oldTeamList.Count > 0)
            {
                int teamId = _oldTeamList[0];
                teamDto = _teamTOModel[teamId];
                _oldTeamList.RemoveAt(0);
                teamDto.LeaderId = playerId;
                teamDto.TeamMembers = roleDTOs;
                teamDto.TeamLevelDown = levelLimint[1];
                teamDto.TeamLevelUp = levelLimint[0];
                teamDto.ApplyMebers = new List<int>();
                _teamTOModel[teamDto.TeamId] = teamDto;
                _playerIdToTeamIdDict.Add(playerId, teamDto.TeamId);

            }
            else
            {
                //TODO 队伍id 需要处理一下
                teamDto = new TeamDTO();
                teamDto.TeamId = teamid++; //_index.GetAndAdd;
                teamDto.LeaderId = playerId;
                teamDto.TeamMembers = roleDTOs;
                teamDto.TeamLevelDown = levelLimint[1];
                teamDto.TeamLevelUp = levelLimint[0];
                teamDto.ApplyMebers = new List<int>();
                _teamTOModel.Add(teamDto.TeamId, teamDto);
                _playerIdToTeamIdDict.Add(playerId, teamDto.TeamId);
                Utility.Debug.LogInfo(teamDto.TeamId + "<>");
            }
        }
        /// <summary>
        /// 加入队伍
        /// </summary>
        /// <param name="roleDTO"></param>
        public void JoinTeam(RoleDTO roleDTO, int teamId)
        {
            if (IsLeader(roleDTO.RoleID)) return;
            ///记得补上 是不是下线
            //if ()
            TeamDTO teamDTO = _teamTOModel[teamId];
            if (teamDTO.TeamMembers.Count > 5) return;
            teamDTO.TeamMembers.Add(roleDTO);  ///将该成员信息加入到队伍成员列表去
            teamDTO.ApplyMebers.Remove(roleDTO.RoleID); //更新申请列表信息
            _teamTOModel[teamId] = teamDTO;
            //List<int> teamMemberIdList = new List<int>();
            //foreach (var dto in teamDTO.TeamMembers)
            //{
            //    teamMemberIdList.Add(dto.RoleID);
            //}
            //DOTO
            //所有组员广播，某人入队信息
        }

        /// <summary>
        /// 加入队伍申请
        /// </summary>
        /// <param name="roleDTO"></param>
        /// <returns></returns>
        public void ApplyJoinTeam(RoleDTO roleDTO, int teamId)
        {

            int playerId = roleDTO.RoleID;
            if (IsLeader(playerId)) //该玩家是不是自己组个队，并且在队伍中
                return;
            TeamDTO teamDTO = _teamTOModel[teamId]; //取出队伍信息
            if (teamDTO.TeamLevelDown < roleDTO.RoleLevel || teamDTO.TeamLevelUp > roleDTO.RoleLevel) //玩家是否符合入队条件。 等级必须够
            {
                Utility.Debug.LogInfo("等级不符合队伍要求");
                return;
            }
            if (teamDTO.ApplyMebers.Contains(roleDTO.RoleID)) return;  //该玩家是不是已经在队中
            teamDTO.ApplyMebers.Add(roleDTO.RoleID);//将玩家加入申请列表
            //TODO 通知队长审核
        }
        /// <summary>
        /// 拒绝队伍申请
        /// </summary>
        public void RefusedApplyTeam(RoleDTO roleDTO, int teamId)
        {
            _teamTOModel[teamId].ApplyMebers.Remove(roleDTO.RoleID);
        }


        /// <summary>
        /// 判断是否是队长
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public bool IsLeader(int playerId)
        {
            if (!_playerIdToTeamIdDict.ContainsKey(playerId))
                return false;
            if (_teamTOModel[_playerIdToTeamIdDict[playerId]].LeaderId != 0)
                return true;
            return false;
        }
        //// <summary>
        /// 返回所有申请入队的人员信息
        /// </summary>
        /// <param name="roleDTO"></param>
        public void GetApplyListInfo(RoleDTO roleDTO)
        {
            if (!IsLeader(roleDTO.RoleID)) return;
            TeamDTO teamDTO = _teamTOModel[_playerIdToTeamIdDict[roleDTO.RoleID]];
            if (teamDTO.ApplyMebers.Count == 0) return; //是否有申请入队的人员
            List<RoleDTO> applyList = new List<RoleDTO>();
            foreach (var id in teamDTO.ApplyMebers)
            {
                //TODO  需要处理一下
                //applyList.Add(id);
            }
        }
        /// <summary>
        /// 返回所有队伍信息
        /// </summary>
        /// <param name="roleDTO"></param>
        public List<TeamDTO> GetTeamList(RoleDTO roleDTO)
        {
            List<TeamDTO> teamDTOs = new List<TeamDTO>();
            foreach (var item in _teamTOModel.Keys)
            {
                if (_teamTOModel[item].LeaderId < 0) continue;
                teamDTOs.Add(_teamTOModel[item]);
            }
            return teamDTOs;
        }

        /// <summary>
        /// 踢人
        /// </summary>
        /// <param name="roleDTO"></param>
        public void KickTeam(RoleDTO roleDTO)
        {
            if (!_playerIdToTeamIdDict.ContainsKey(roleDTO.RoleID))
            {
                Utility.Debug.LogInfo("你当前没有权限！");
                return;
            }

            TeamDTO teamDTO = _teamTOModel[_playerIdToTeamIdDict[roleDTO.RoleID]];
            for (int i = 0; i < teamDTO.TeamMembers.Count; i++)
            {
                if (teamDTO.TeamMembers[i].RoleID == roleDTO.RoleID)
                {
                    teamDTO.TeamMembers.RemoveAt(i);
                    Utility.Debug.LogInfo(" 你被请离了队伍！！");
                    break;
                }
            }

            List<int> teamMemberIdList = new List<int>();

            foreach (var dto in teamDTO.TeamMembers)
            {
                teamMemberIdList.Add(dto.RoleID);
            }
            Utility.Debug.LogInfo(" 离开了队伍！");
        }
        /// <summary>
        /// 离开队伍
        /// </summary>
        /// <param name="roleDTO"></param>
        public void LeaveTeam(RoleDTO roleDTO)
        {
            int teamId = _playerIdToTeamIdDict[roleDTO.RoleID];
            TeamDTO teamDTO = _teamTOModel[teamId];
            if (teamDTO.LeaderId == roleDTO.RoleID)
            {
                //队长离开队伍
                if (teamDTO.TeamMembers.Count > 1)
                {
                    _playerIdToTeamIdDict.Remove(roleDTO.RoleID);
                    //房间还有其他人
                    teamDTO.TeamMembers.RemoveAt(0);
                    teamDTO.LeaderId = teamDTO.TeamMembers[0].RoleID;
                    _playerIdToTeamIdDict.Add(teamDTO.TeamMembers[0].RoleID, teamId);
                    List<int> teamMemberIdList = new List<int>();
                    foreach (var dto in teamDTO.TeamMembers)
                    {
                        teamMemberIdList.Add(dto.RoleID);
                    }
                    //TODO
                    Utility.Debug.LogInfo(" 你被提升为队长！");
                    Utility.Debug.LogInfo(" 你离开了队伍！");
                }
                else
                {
                    //房间只有自己
                    _playerIdToTeamIdDict.Remove(roleDTO.RoleID);
                    _oldTeamList.Add(teamDTO.TeamId);
                    teamDTO.LeaderId = 0;
                    teamDTO.TeamMembers.Clear();
                    teamDTO.TeamLevelDown = 0;
                    teamDTO.TeamLevelUp = 99;
                    Utility.Debug.LogInfo(" 你离开了队伍！");
                }
            }
            else
            {
                //队员离开队伍
                for (int i = 0; i < teamDTO.TeamMembers.Count; i++)
                {
                    if (teamDTO.TeamMembers[i].RoleID == roleDTO.RoleID)
                    {
                        teamDTO.TeamMembers.RemoveAt(i);
                        break;
                    }
                }
                Utility.Debug.LogInfo(" 你离开了队伍！");
                List<int> teamMemberIdList = new List<int>();
                foreach (var dto in teamDTO.TeamMembers)
                {
                    teamMemberIdList.Add(dto.RoleID);
                }
            }
            GetTeamList(roleDTO);
        }
    }
}