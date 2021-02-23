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
            RoomidToTimer[roomId].StartTimer();
        }
        /// <summary>
        /// 针对初始化准备加载 倒计时
        /// </summary>
        /// <param name="teamId"></param>
        public void TimestampBattlePrepare(int teamId)
        {
            TeamidToTimer[teamId].PrepareTimer();
        }
        /// <summary>
        /// 针对组队 开始之前倒计时
        /// </summary>
        /// <param name="teamId"></param>
        public void TimestampBattleStart(int teamId)
        {
            TeamidToTimer[teamId].BattleStartTimer();
        }

        /// <summary>
        ///每个回合倒计时 AI 玩家 是否死亡 战斗结束 发起事件 
        /// </summary>
        public void BattleIsDieCallBack(int roomId)
        {
            var tempRoleId =TeamIdToBattleInit.FirstOrDefault(t => t.Value.RoomId == roomId).Key;
            // TeamIdToBattleInit[tempRoleId].enemyUnits.Find(t => t.EnemyStatusDTO.EnemyHP == 0).EnemyStatusDTO;
            //Utility.Debug.LogInfo("老陆   roleStatusSever"+ roleStatusSever.RoleHP);
            if (IsTeamDto(tempRoleId) == null)
            {
                var roleStatusSever = TeamIdToBattleInit[tempRoleId].playerUnits[0].RoleStatusDTO;
                var allEnemyHP = TeamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                if ((roleStatusSever.RoleHP <= 0 && (TeamIdToBattleInit[tempRoleId].petUnits.Count != 0 ? TeamIdToBattleInit[tempRoleId].petUnits[0].PetStatusDTO.PetHP <= 0 : true)) || allEnemyHP == null)
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = "战斗结束啦， over！";
                    opData.OperationCode = (byte)OperationCode.SyncBattleMessageEnd;
                   GameEntry. RoleManager.SendMessage(tempRoleId, opData);
                    TeamIdToBattleInit.Remove(tempRoleId);
                    //BattleEnd(roomId);
                }
                else
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = "服务器 倒计时10 秒  over！ ";
                    opData.OperationCode = (byte)OperationCode.SyncBattleRound;
                    GameEntry.RoleManager.SendMessage(tempRoleId, opData);
                }
            }
            else
            {
                if (GameEntry.ServerTeamManager.TeamTOModel.ContainsKey(IsTeamDto(tempRoleId).TeamId))
                {
                    var allRoleHP = TeamIdToBattleInit[tempRoleId].playerUnits.Find(q => q.RoleStatusDTO.RoleHP >= 0);
                    var allEnemyHP =TeamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                    //Utility.Debug.LogInfo("战斗结束" + allRoleHP.RoleStatusDTO.RoleHP + "<>" + allEnemyHP.EnemyStatusDTO.EnemyHP);

                    for (int ob = 0; ob < GameEntry.ServerTeamManager.TeamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers.Count; ob++)
                    {
                        // if (TeamIdToBattleInit[tempRoleId].playerUnits[ob].RoleStatusDTO.RoleHP <= 0)
                        if ((allRoleHP == null&&(TeamIdToBattleInit[tempRoleId].petUnits.Count != 0 ? TeamIdToBattleInit[tempRoleId].petUnits.Find(x=>x.PetStatusDTO.PetHP>0) == null : true)) || allEnemyHP == null)
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = "战斗结束啦， over！";
                            opData.OperationCode = (byte)OperationCode.SyncBattleMessageEnd;
                           GameEntry. RoleManager.SendMessage(GameEntry. ServerTeamManager.TeamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers[ob].RoleID, opData);
                            TeamIdToBattleInit.Remove(tempRoleId);
                            ///战斗结束 同步血量
                            //BattleEnd(roomId);
                        }
                        else
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = "服务器 倒计时10 秒  over！ ";
                            opData.OperationCode = (byte)OperationCode.SyncBattleRound;
                            GameEntry.RoleManager.SendMessage(GameEntry. ServerTeamManager.TeamTOModel[IsTeamDto(tempRoleId).TeamId].TeamMembers[ob].RoleID, opData);
                        }
                    }
                    //Utility.Debug.LogInfo("老陆 , 每回合倒计时结束 ！===>"+ _teamidToTimer.Count);
                    TeamidToTimer.Add(IsTeamDto(tempRoleId).TeamId, new TimerToManager(10000));
                    TimestampBattleStart(IsTeamDto(tempRoleId).TeamId);
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
            if (TeamIdToMemberDict.ContainsKey(tempTeamId))
            {
                for (int i = 0; i < GameEntry.ServerTeamManager.TeamTOModel[tempTeamId].TeamMembers.Count; i++)
                {
                    OperationData opData = new OperationData();
                    opData.DataMessage = TeamIdToMemberDict[tempTeamId].Count + "个人服务器 组队 准备完成， over！";
                    opData.OperationCode = (byte)OperationCode.SyncBattleMessagePrepare;
                    GameEntry.RoleManager.SendMessage(GameEntry.ServerTeamManager.TeamTOModel[tempTeamId].TeamMembers[i].RoleID, opData);
                }
                TeamidToTimer.Remove(tempTeamId);
                //GameEntry.ServerBattleManager._teamIdToMemberDict.Remove(tempTeamId);
            }
        }


        /// <summary>
        /// 针对 组队情况下的 不选取指令  随机分配一个默认指令     ??? 需要处理 不发消息的时候怎么办
        /// </summary>
        public void RoundTeamMember(int teampRoomId,int tempTeamId,int tempRole)
        {
            var serverBattleManager = GameEntry.ServerBattleManager;
            var serverTeamManager = GameEntry.ServerTeamManager;

            for (int i = 0; i < serverTeamManager.TeamTOModel[tempTeamId].TeamMembers.Count; i++)
            {
                if (serverBattleManager.RoomidToBattleTransfer[teampRoomId].Find(x => x.RoleId == serverTeamManager.TeamTOModel[tempTeamId].TeamMembers[i].RoleID) != null)
                    continue;
                //serverBattleManager._teamIdToMemberDict[tempTeamId].Add(serverTeamManager._teamTOModel[tempTeamId].TeamMembers[i].RoleID);
                //TODO  ////默认是用第一个战斗传输的数据
                BattleTransferDTO battleTransfer = new BattleTransferDTO();
                BattleTransferDTO battleTransferPet = new BattleTransferDTO();
                battleTransfer.RoleId = serverTeamManager.TeamTOModel[tempTeamId].TeamMembers[i].RoleID;
                battleTransfer.isFinish = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].isFinish;
                battleTransfer.BattleCmd = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].BattleCmd;
                battleTransfer.ClientCmdId = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].ClientCmdId;
                battleTransfer.TargetInfos = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].TargetInfos;
                battleTransfer.SkillReactionValue = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].SkillReactionValue;
                battleTransfer.SendSkillReactionCmd = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].SendSkillReactionCmd;
                battleTransfer.RoleIdShieldValueDict = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].RoleIdShieldValueDict;
                if (serverBattleManager.TeamIdToBattleInit[tempRole].petUnits.Count != 0)
                {
                    PetBattleDataDTO petObject = serverBattleManager.TeamIdToBattleInit[tempRole].petUnits.Find(x => x.RoleId == serverTeamManager.TeamTOModel[tempTeamId].TeamMembers[i].RoleID);
                    if (petObject != null)
                    {
                        battleTransferPet.RoleId = petObject.PetStatusDTO.PetID;
                        battleTransferPet.isFinish = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.isFinish;
                        battleTransferPet.BattleCmd = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.BattleCmd;
                        battleTransferPet.ClientCmdId = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.ClientCmdId;
                        battleTransferPet.TargetInfos = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.TargetInfos;
                        battleTransferPet.SkillReactionValue = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.SkillReactionValue;
                        battleTransferPet.SendSkillReactionCmd = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.SendSkillReactionCmd;
                        battleTransferPet.RoleIdShieldValueDict = serverBattleManager.RoomidToBattleTransfer[teampRoomId][0].petBattleTransferDTO.RoleIdShieldValueDict;
                        battleTransfer.petBattleTransferDTO = battleTransferPet;
                    }
                }
                serverBattleManager.RoomidToBattleTransfer[teampRoomId].Add(battleTransfer);
            }
        }


        /// <summary>
        /// 针对每回合组队 技能释放计算 并返回给客户端
        /// </summary>
        /// <param name="tempRole"></param>
        /// <param name="teampRoomId"></param>
        /// <param name="tempTeamId"></param>
        /// 
        public int IsTeamRunAway { get; set; }
        public  bool IsPetTeamRunAway { get; set; }
        public void RoundTeamSkillComplete(int tempRole,int teampRoomId , int tempTeamId)
        {
            IsTeamRunAway = 0;
            IsPetTeamRunAway  = false;
            int playerTemp = TeamIdToBattleInit[tempRole].playerUnits.Count;
            var serverBattleManager = GameEntry.ServerBattleManager;
            var serverTeamManager = GameEntry.ServerTeamManager;
            for (int speed = 0; speed < serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits.Count; speed++)
            {
                if (IsTeamRunAway == playerTemp)
                    break;
                var objectOwner = serverBattleManager.ReleaseToOwner(serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId, tempRole);
                if (objectOwner == null)
                    continue;
                var typeName = objectOwner.GetType().Name;
                switch (typeName)
                {
                    case "EnemyStatusDTO":
                        var enemyStatusData = objectOwner as EnemyStatusDTO;
                        //Utility.Debug.LogInfo("剔除一个 ====》" + serverBattleManager.RoomidToBattleTransfer[teampRoomId].Count);
                        ///返回一个当前要出手的人的个人属性   需要判断TODO
                        if (serverBattleManager.RoomidToBattleTransfer[teampRoomId].Count  ==0)
                            break;
                        var EnemyIndex = RandomManager(speed, 0, serverBattleManager.RoomidToBattleTransfer[teampRoomId].Count); 
                        var memberCuuentTranferEnemy = serverBattleManager.TeamIdToBattleInit[tempRole].playerUnits.Find(x => x.RoleStatusDTO.RoleID == serverBattleManager.RoomidToBattleTransfer[teampRoomId][EnemyIndex].RoleId);

                        if (enemyStatusData.EnemyHP > 0 && memberCuuentTranferEnemy.RoleStatusDTO.RoleHP > 0)
                            serverBattleManager.AIToRelease(serverBattleManager.RoomidToBattleTransfer[teampRoomId][EnemyIndex], enemyStatusData, tempRole, memberCuuentTranferEnemy.RoleStatusDTO.RoleID, EnemyIndex);
                        break;
                    case "RoleStatusDTO":
                        ///返回一个当前要出手的人的个人选择的传输的战斗数据
                        var speedCuurentTransfer = serverBattleManager.RoomidToBattleTransfer[teampRoomId].Find(q => q.RoleId == serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                        ///返回一个当前要出手的人的个人属性
                        var memberCuuentTranfer = serverBattleManager.TeamIdToBattleInit[tempRole].playerUnits.Find(x => x.RoleStatusDTO.RoleID == serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                        var memberCuuentTranferIndex = serverBattleManager.TeamIdToBattleInit[tempRole].playerUnits.FindIndex(x => x.RoleStatusDTO.RoleID == serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                        switch (speedCuurentTransfer.BattleCmd)
                        {
                            #region 针对道具
                            case BattleCmd.PropsInstruction:
                                if (memberCuuentTranfer.RoleStatusDTO.RoleHP > 0)
                                    serverBattleManager.PlayerToPropslnstruction(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                                break;
                            #endregion
                            #region 针对技能
                            case BattleCmd.SkillInstruction:
                                if (memberCuuentTranfer.RoleStatusDTO.RoleHP > 0)
                                    PlayerToSKillRelease(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID, memberCuuentTranferIndex);
                                break;
                            #endregion
                            #region 针对逃跑 需要完善
                            case BattleCmd.RunAwayInstruction:
                                PlayerTeamToRunAway(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID, memberCuuentTranferIndex, speed);
                                break;
                            #endregion
                            #region 针对法宝
                            case BattleCmd.MagicWeapon:
                                if (memberCuuentTranfer.RoleStatusDTO.RoleHP > 0)
                                    serverBattleManager.PlayerToMagicWeapen(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                                break;
                            #endregion
                            #region 针对捕捉
                            case BattleCmd.CatchPet:
                                if (MonsterFormToObject(speedCuurentTransfer.TargetInfos[0].GlobalId) != null)
                                {
                                    var targetOwner = MonsterFormToObject(speedCuurentTransfer.TargetInfos[0].GlobalId);
                                    PlayerToCatchPet(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID, targetOwner);
                                }
                                break;
                            #endregion
                            #region 针对召唤
                            case BattleCmd.SummonPet:
                                break;
                            #endregion
                            case BattleCmd.Tactical:
                                break;
                            #region 针对防御
                            case BattleCmd.Defend:
                                PlayerToDefend(speedCuurentTransfer, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectID);
                                break;
                                #endregion
                        }
                        break;
                    case "PetStatusDTO":
                        var petStatusDTO = objectOwner as PetStatusDTO;
                        if (petStatusDTO.PetHP <= 0)
                            continue;
                        var speedCuurentTransferPet = serverBattleManager.RoomidToBattleTransfer[teampRoomId].Find(q => q.petBattleTransferDTO.RoleId == serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId);
                        var memberCuuentTranferIndexPet = serverBattleManager.TeamIdToBattleInit[tempRole].petUnits.FindIndex(x => x.PetStatusDTO.PetID == serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId);
                        switch (speedCuurentTransferPet.petBattleTransferDTO.BattleCmd)
                        {
                            #region 宠物道具
                            case BattleCmd.PropsInstruction:
                                PlayerToPropslnstruction(speedCuurentTransferPet.petBattleTransferDTO, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId);
                                break;
                            #endregion
                            #region 宠物技能
                            case BattleCmd.SkillInstruction:
                                PlayerToSKillRelease(speedCuurentTransferPet.petBattleTransferDTO, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId, memberCuuentTranferIndexPet);
                                break;
                            #endregion
                            #region 宠物逃跑
                            case BattleCmd.RunAwayInstruction:
                                PlayerTeamToRunAway(speedCuurentTransferPet.petBattleTransferDTO, tempRole, serverBattleManager.TeamIdToBattleInit[tempRole].battleUnits[speed].ObjectId, memberCuuentTranferIndexPet, -1);
                                break;
                            #endregion
                            default:
                                break;
                        }
                        break;
                }
            }
            //Utility.Debug.LogInfo("需要发给的人=====》》" + serverBattleManager._teamIdToMemberDict[tempTeamId].Count);
            ///通知所有玩家当前回合战斗计算完毕
            for (int op = 0; op < serverTeamManager.TeamTOModel[tempTeamId].TeamMembers.Count; op++)
            {
                Utility.Debug.LogInfo("发给客户端" + serverTeamManager.TeamTOModel[tempTeamId].TeamMembers[op].RoleID);
                OperationData opData = new OperationData();
                opData.DataMessage = serverBattleManager.RoundServerToClient();
                opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
               GameEntry. RoleManager.SendMessage(serverTeamManager.TeamTOModel[tempTeamId].TeamMembers[op].RoleID, opData);
            }
        }
      
        #endregion
    }
}


