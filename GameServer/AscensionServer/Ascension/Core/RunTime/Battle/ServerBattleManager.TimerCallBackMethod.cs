using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 处理所有关于倒计时的的开始  和 倒计时结束的时候的回调
/// </summary>
namespace AscensionServer
{
   public partial class ServerBattleManager
    {
        #region 所有关于倒计时的开始和回调事件

        //public TimerManager timer;
        /// <summary>
        ///针对每回合  开始倒计时
        /// </summary>
        public void TimestampBattleEnd(int roomId)
        {
            _roomidToTimer[roomId].StartTimer();
        }
        /// <summary>
        /// 针对初始化准备加载 倒计时
        /// </summary>
        /// <param name="teamId"></param>
        public void TimestampBattlePrepare(int teamId)
        {
            _teamidToTimer[teamId].PrepareTimer();
        }
        /// <summary>
        /// 针对组队 开始之前倒计时
        /// </summary>
        /// <param name="teamId"></param>
        public void TimestampBattleStart(int teamId)
        {
            _teamidToTimer[teamId].BattleStartTimer();
        }

        /// <summary>
        ///每个回合倒计时 AI 玩家 是否死亡 战斗结束 发起事件 
        /// </summary>
        public void BattleIsDieCallBack(int roomId)
        {
            var tempRoleId = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit.FirstOrDefault(t => t.Value.RoomId == roomId).Key;
            // _teamIdToBattleInit[tempRoleId].enemyUnits.Find(t => t.EnemyStatusDTO.EnemyHP == 0).EnemyStatusDTO;
            //Utility.Debug.LogInfo("老陆   roleStatusSever"+ roleStatusSever.RoleHP);
            if (IsTeamDto(tempRoleId) == null)
            {
                var roleStatusSever = _teamIdToBattleInit[tempRoleId].playerUnits[0].RoleStatusDTO;
                var allEnemyHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                if (roleStatusSever.RoleHP <= 0 || allEnemyHP == null)
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = "战斗结束啦， over！";
                    opData.OperationCode = (byte)OperationCode.SyncBattleMessageEnd;
                    GameManager.CustomeModule<RoleManager>().SendMessage(tempRoleId, opData);
                    //BattleEnd(roomId);
                }
                else
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = "服务器 倒计时10 秒  over！ ";
                    opData.OperationCode = (byte)OperationCode.SyncBattleRound;
                    GameManager.CustomeModule<RoleManager>().SendMessage(tempRoleId, opData);
                }
            }
            else
            {
                if (GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.ContainsKey(IsTeamDto(tempRoleId).TeamId))
                {
                    var allRoleHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].playerUnits.Find(q => q.RoleStatusDTO.RoleHP >= 0);
                    var allEnemyHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                    //Utility.Debug.LogInfo("战斗结束" + allRoleHP.RoleStatusDTO.RoleHP + "<>" + allEnemyHP.EnemyStatusDTO.EnemyHP);

                    for (int ob = 0; ob < GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers.Count; ob++)
                    {
                        // if (_teamIdToBattleInit[tempRoleId].playerUnits[ob].RoleStatusDTO.RoleHP <= 0)
                        if (allRoleHP == null || allEnemyHP == null)
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = "战斗结束啦， over！";
                            opData.OperationCode = (byte)OperationCode.SyncBattleMessageEnd;
                            GameManager.CustomeModule<RoleManager>().SendMessage(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers[ob].RoleID, opData);
                            ///战斗结束 同步血量
                            //BattleEnd(roomId);
                        }
                        else
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = "服务器 倒计时10 秒  over！ ";
                            opData.OperationCode = (byte)OperationCode.SyncBattleRound;
                            GameManager.CustomeModule<RoleManager>().SendMessage(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers[ob].RoleID, opData);
                        }
                    }
                    //Utility.Debug.LogInfo("老陆 , 每回合倒计时结束 ！===>"+ _teamidToTimer.Count);
                    GameManager.CustomeModule<ServerBattleManager>()._teamidToTimer.Add(IsTeamDto(tempRoleId).TeamId, new TimerToManager(10000));
                    GameManager.CustomeModule<ServerBattleManager>().TimestampBattleStart(IsTeamDto(tempRoleId).TeamId);
                }
            }
        }
        /// <summary>
        /// 针对组队 战斗准备阶段倒计时  回调事件
        /// </summary>
        /// <param name="tempTeamId"></param>
        public void BattleTimerPrepareCallBack(int tempTeamId)
        {
            //TODO
            if (GameManager.CustomeModule<ServerBattleManager>()._teamIdToMemberDict.ContainsKey(tempTeamId))
            {
                for (int i = 0; i < GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[tempTeamId].TeamMembers.Count; i++)
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = GameManager.CustomeModule<ServerBattleManager>()._teamIdToMemberDict[tempTeamId].Count + "个人服务器 组队 准备完成， over！";
                    opData.OperationCode = (byte)OperationCode.SyncBattleMessagePrepare;
                    GameManager.CustomeModule<RoleManager>().SendMessage(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel[tempTeamId].TeamMembers[i].RoleID, opData);
                }

                GameManager.CustomeModule<ServerBattleManager>()._teamidToTimer.Remove(tempTeamId);
                //GameManager.CustomeModule<ServerBattleManager>()._teamIdToMemberDict.Remove(tempTeamId);
            }
        }
        #endregion
    }
}
