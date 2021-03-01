using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AscensionServer
{
    /// <summary>
    /// 没统一的服务器战斗功能
    /// </summary>
    [Module]
    public partial class ServerBattleManager :Cosmos. Module,IServerBattleManager
    {
        /*
         * 
         * 
         * 
         * */
        /// <summary>
        /// 注意 每次进去之前先保证之前的数据都清除掉
        /// </summary>
        /// <param name="roleId"></param>
        public void BattleClear(int roleId)
        {
            TeamSet = new List<BattleTransferDTO>();
            BuffToRoomIdBefore = new List<BattleBuffDTO>();
            BuffToRoomIdAfter = new List<BattleBuffDTO>();
            if (TeamidToTimer.ContainsKey(IsTeamDto(roleId).TeamId))
                TeamidToTimer.Remove(IsTeamDto(roleId).TeamId);
            if (TeamIdToMemberDict.ContainsKey(IsTeamDto(roleId).TeamId))
                TeamIdToMemberDict.Remove(IsTeamDto(roleId).TeamId);
            TargetID.Clear();
            TeamSet.Clear();
            BuffToRoomIdBefore.Clear();
            BuffToRoomIdAfter.Clear();
        }

        /// <summary>
        /// 初始化战斗数据  
        /// </summary>
        public void EntryBattle(BattleInitDTO battleInitDTO)
        {
            var team =GameEntry.ServerTeamManager.TeamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID) != null);
            if (team != null && team.LeaderId != battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID)
            {
                Utility.Debug.LogInfo("有自己的队伍,但不是队长！！");
                return;
            }

            BattleInitDTO battleInit;
            if (OldBattleList.Count > 0)
            {
                //int roomid = _oldBattleList[0];
                //battleInit = _teamIdToBattleInit[roomid];
            }
            else
            {
                battleInit = new BattleInitDTO();
                battleInit.RoomId = _roomId++;
                battleInit.countDownSec = 10;//battleInitDTO.countDownSec;
                battleInit.roundCount = battleInitDTO.roundCount;
                battleInit.playerUnits = RoleInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.petUnits =PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);//battleInitDTO.petUnits;
                battleInit.enemyUnits = EnemyInfo(battleInitDTO.enemyUnits);
                battleInit.enemyPetUnits = battleInitDTO.enemyPetUnits;
                battleInit.maxRoundCount = battleInitDTO.maxRoundCount;
                battleInit.battleUnits = AllBattleDataDTOsInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInitDTO);
                //battleInit.bufferUnits = new List<BufferBattleDataDTO>();
                TeamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                TeamIdToBattleInitData.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                RoomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());
                RoomidToTimer.Add(battleInit.RoomId, new TimerToManager(RoleBattleTime));
            }
            InitBattle(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
            BattleClear(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
        }


        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        public void PrepareBattle(int roleId,int roomId)
        {
            if (IsTeamDto(roleId) == null)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = roleId + "=>个人服务器  准备完成， over！";
                opData.OperationCode = (byte)OperationCode.SyncBattleMessagePrepare;
                GameEntry.RoleManager.SendMessage(roleId, opData);
            }
            else
            {
                //判断当前队伍
                if (TeamidToTimer.ContainsKey(IsTeamDto(roleId).TeamId))
                {
                    TeamIdToMemberDict[IsTeamDto(roleId).TeamId].Add(roleId);
                    return;
                }
                
                if ( RecordTeamId.Contains(IsTeamDto(roleId).TeamId))
                     RecordTeamId.ToList().Remove(IsTeamDto(roleId).TeamId);
                 RecordTeamId.Enqueue(IsTeamDto(roleId).TeamId);
                TeamidToTimer.Add(IsTeamDto(roleId).TeamId, new TimerToManager(6000));
                List<int> memberSet = new List<int>();
                memberSet.Add(roleId);
                TeamIdToMemberDict.Add(IsTeamDto(roleId).TeamId, memberSet);
                 TimestampBattlePrepare(IsTeamDto(roleId).TeamId);
                if (RoomidToBattleTransfer.ContainsKey(roomId))
                    RoomidToBattleTransfer[roomId] = new List<BattleTransferDTO>();
            }
        }


        /// <summary>
        /// 开始战斗   -->  开始战斗的回合
        /// </summary>
        /// 
        bool isRunAway;
        bool isPetRunAway;
        int tempSelect;
        public void BattleStart(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            //Utility.Debug.LogInfo("battleCmd == >>>" + battleTransferDTOs.BattleCmd);
            TargetID.Clear();
            TeamSet.Clear();
            BuffToRoomIdBefore.Clear();
            BuffToRoomIdAfter.Clear();
            isRunAway = false;
            isPetRunAway = false;
            tempSelect = 0;
            if (!RoomidToBattleTransfer.ContainsKey(roomId))
                return;

            if (IsTeamDto(roleId) == null)
            {
                ReleaseToSpeed(roleId);
                ///出手速度
                for (int speed = 0; speed < BattleInitObject(roleId).battleUnits.Count; speed++)
                {
                    if (isRunAway)
                        break;
                    var objectOwner = ReleaseToOwner(BattleInitObject(roleId).battleUnits[speed].ObjectID, BattleInitObject(roleId).battleUnits[speed].ObjectId, roleId);
                    if (objectOwner == null || tempSelect == 1)
                        continue;
                    var typeName = objectOwner.GetType().Name;
                    //Utility.Debug.LogInfo("角色剩余的血量" + BattleInitObject(roleId).playerUnits[0].RoleStatusDTO.RoleHP);
                    switch (typeName)
                    {
                        case "EnemyStatusDTO":
                            var enemyStatusData = objectOwner as EnemyStatusDTO;
                            if (enemyStatusData.EnemyHP > 0 && BattleInitObject(roleId).playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                AIToRelease(battleTransferDTOs, enemyStatusData, roleId,roleId,speed);
                            break;
                        case "RoleStatusDTO":
                            if (BattleInitObject(roleId).playerUnits[0].RoleStatusDTO.RoleHP <= 0)
                                break;
                            switch (battleTransferDTOs.BattleCmd)
                            {
                                #region 针对道具
                                case BattleCmd.PropsInstruction:
                                    PlayerToPropslnstruction(battleTransferDTOs, roleId, roleId);
                                    break;
                                #endregion
                                #region 针对技能
                                case BattleCmd.SkillInstruction:
                                    PlayerToSKillRelease(battleTransferDTOs, roleId, roleId);
                                    break;
                                #endregion
                                #region 针对逃跑
                                case BattleCmd.RunAwayInstruction:
                                    PlayerToRunAway(battleTransferDTOs, roleId);
                                    break;
                                #endregion
                                #region 针对法宝
                                case BattleCmd.MagicWeapon:
                                    PlayerToMagicWeapen(battleTransferDTOs, roleId, roleId);
                                    break;
                                #endregion
                                #region 针对捕捉 
                                case BattleCmd.CatchPet:
                                    if (MonsterFormToObject(battleTransferDTOs.TargetInfos[0].GlobalId) != null)
                                    {
                                        var targetOwner = MonsterFormToObject(battleTransferDTOs.TargetInfos[0].GlobalId);
                                        PlayerToCatchPet(battleTransferDTOs, roleId, roleId, targetOwner);
                                    }
                                    break;
                                #endregion
                                #region 针对召唤
                                case BattleCmd.SummonPet:
                                    PlayerToSummonPet(battleTransferDTOs, roleId, roleId);
                                    break;
                                #endregion
                                case BattleCmd.Tactical:
                                    break;
                                #region 针对防御
                                case BattleCmd.Defend:
                                    PlayerToDefend(battleTransferDTOs, roleId, roleId);
                                    break;
                                    #endregion
                            }
                            break;
                        case "PetStatusDTO":
                            var petStatusDTO = objectOwner as PetStatusDTO;
                            if (petStatusDTO.PetHP <= 0)
                                continue;
                            //Utility.Debug.LogInfo("老陆 测试" + BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetHP);
                            switch (battleTransferDTOs.petBattleTransferDTO.BattleCmd)
                            {
                                #region 宠物道具
                                case BattleCmd.PropsInstruction:
                                    if (BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetHP > 0)
                                        PlayerToPropslnstruction(battleTransferDTOs.petBattleTransferDTO, roleId, battleTransferDTOs.petBattleTransferDTO.RoleId);
                                    break;
                                #endregion
                                #region 宠物技能
                                case BattleCmd.SkillInstruction:
                                    if (BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetHP > 0)
                                        PlayerToSKillRelease(battleTransferDTOs.petBattleTransferDTO, roleId, battleTransferDTOs.petBattleTransferDTO.RoleId);
                                    break;
                                #endregion
                                #region 宠物逃跑
                                case BattleCmd.RunAwayInstruction:
                                    PetToRunAway(battleTransferDTOs.petBattleTransferDTO, roleId, battleTransferDTOs.petBattleTransferDTO.RoleId);
                                    break;
                                #endregion
                                default:
                                    break;
                            }
                            break;
                    }
                }
                SkillActionDifferentCmd(battleTransferDTOs.BattleCmd, roleId,roomId);
            }
            else
            {
                if (RoomidToBattleTransfer.ContainsKey(roomId))
                    RoomidToBattleTransfer[roomId].Add(battleTransferDTOs);
                if (!RecordTeamRooomId.Contains(roomId))
                {
                    if (!TeamIdToRoomId.ContainsKey(IsTeamDto(roleId).TeamId))
                        TeamIdToRoomId.Add(IsTeamDto(roleId).TeamId, roomId);
                    RecordTeamRooomId.Enqueue(roomId);
                }
                Utility.Debug.LogInfo("老陆 ，开始战斗的时候收集客户端一个请求"+ RoomidToBattleTransfer[roomId].Count);
            }
        }


        /// 战斗结束
        /// </summary>
        public void BattleEnd(int RoomId)
        {
            var tempRoleId =  TeamIdToBattleInit.FirstOrDefault(t => t.Value.RoomId == RoomId).Key;
            var roleStatusSever = TeamIdToBattleInit[tempRoleId].playerUnits[0].RoleStatusDTO;
            PlayerBattleEndInfo(tempRoleId, roleStatusSever);
        }



        /// <summary>
        /// 战斗逃跑
        /// </summary>
        public void BattleRunAway(int roleId,int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.RunAwayInstruction)
                BattleStart(roleId, roomId, battleTransferDTOs);
        }
        /// <summary>
        /// 战斗道具
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        public void BattlePropsInstrution(int roleId,int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction)
                BattleStart(roleId, roomId, battleTransferDTOs); ;
        }
        /// <summary>
        /// 战斗法宝
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        public void BattleMagicWeapen(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon)
                BattleStart(roleId, roomId, battleTransferDTOs); 
        }

        /// <summary>
        /// 战斗捕捉
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
       public void BattleCatchPet(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.CatchPet)
                BattleStart(roleId, roomId, battleTransferDTOs);
        }
       /// <summary>
       /// 战斗召唤
       /// </summary>
       /// <param name="roleId"></param>
       /// <param name="roomId"></param>
       /// <param name="battleTransferDTOs"></param>
        public void BattleSummonPet(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.SummonPet)
                BattleStart(roleId, roomId, battleTransferDTOs);
        }
        /// <summary>
        /// 战斗防御指令
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        /// <param name="battleTransferDTOs"></param>
        public void BattleDefend(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            if (battleTransferDTOs.BattleCmd == BattleCmd.Defend)
                BattleStart(roleId, roomId, battleTransferDTOs);
        }

    }
}


