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
    [CustomeModule]
    public partial class ServerBattleManager : Module<ServerBattleManager>
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
            if (_teamidToTimer.ContainsKey(IsTeamDto(roleId).TeamId))
                _teamidToTimer.Remove(IsTeamDto(roleId).TeamId);
            if (_teamIdToMemberDict.ContainsKey(IsTeamDto(roleId).TeamId))
                _teamIdToMemberDict.Remove(IsTeamDto(roleId).TeamId);
            TargetID.Clear();
            teamSet.Clear();
        }

        /// <summary>
        /// 初始化战斗数据  
        /// </summary>
        public void EntryBattle(BattleInitDTO battleInitDTO)
        {
            var team = GameManager.CustomeModule<ServerTeamManager>()._teamTOModel.Values.ToList().Find(x => x.TeamMembers.Find(q => q.RoleID == battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID) != null);
            if (team != null && team.LeaderId != battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID)
            {
                Utility.Debug.LogInfo("有自己的队伍,但不是队长！！");
                return;
            }

            BattleInitDTO battleInit;
            if (_oldBattleList.Count > 0)
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
                battleInit.petUnits = PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);//battleInitDTO.petUnits;
                battleInit.enemyUnits = EnemyInfo(battleInitDTO.enemyUnits);
                battleInit.enemyPetUnits = battleInitDTO.enemyPetUnits;// PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.maxRoundCount = battleInitDTO.maxRoundCount;
                battleInit.battleUnits = AllBattleDataDTOsInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInitDTO);
                _teamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());
                _roomidToTimer.Add(battleInit.RoomId, new TimerToManager(RoleBattleTime));
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
                GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
            }
            else
            {
                //判断当前队伍
                if (_teamidToTimer.ContainsKey(IsTeamDto(roleId).TeamId))
                {
                    _teamIdToMemberDict[IsTeamDto(roleId).TeamId].Add(roleId);
                    return;
                }
                
                if (GameManager.CustomeModule<ServerBattleManager>().RecordTeamId.Contains(IsTeamDto(roleId).TeamId))
                    GameManager.CustomeModule<ServerBattleManager>().RecordTeamId.ToList().Remove(IsTeamDto(roleId).TeamId);
                 GameManager.CustomeModule<ServerBattleManager>().RecordTeamId.Enqueue(IsTeamDto(roleId).TeamId);
                _teamidToTimer.Add(IsTeamDto(roleId).TeamId, new TimerToManager(6000));
                List<int> memberSet = new List<int>();
                memberSet.Add(roleId);
                _teamIdToMemberDict.Add(IsTeamDto(roleId).TeamId, memberSet);
                GameManager.CustomeModule<ServerBattleManager>().TimestampBattlePrepare(IsTeamDto(roleId).TeamId);
                if (_roomidToBattleTransfer.ContainsKey(roomId))
                    _roomidToBattleTransfer[roomId] = new List<BattleTransferDTO>();
            }
        }


        /// <summary>
        /// 开始战斗   -->  开始战斗的回合
        /// </summary>
        public void BattleStart(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {

            //Utility.Debug.LogInfo("battleCmd == >>>" + battleTransferDTOs.BattleCmd);
            TargetID.Clear();
            teamSet.Clear();
            bool isRunAway = false;
            if (!_roomidToBattleTransfer.ContainsKey(roomId))
                return;

            if (IsTeamDto(roleId) == null)
            {
                ReleaseToSpeed(roleId);
                ///出手速度
                for (int speed = 0; speed < _teamIdToBattleInit[roleId].battleUnits.Count; speed++)
                {
                    var objectOwner = ReleaseToOwner(_teamIdToBattleInit[roleId].battleUnits[speed].ObjectID, _teamIdToBattleInit[roleId].battleUnits[speed].ObjectId, roleId);
                    var typeName = objectOwner.GetType().Name;
                    //Utility.Debug.LogInfo("老陆 测试" + typeName);
                    //Utility.Debug.LogInfo("角色剩余的血量" + _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP);
                    switch (typeName)
                    {
                        case "EnemyStatusDTO":
                            var enemyStatusData = objectOwner as EnemyStatusDTO;
                            if (isRunAway)
                                break;
                            if (enemyStatusData.EnemyHP > 0 && _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                AIToRelease(battleTransferDTOs, enemyStatusData, roleId);
                            break;
                        case "RoleStatusDTO":
                            switch (battleTransferDTOs.BattleCmd)
                            {
                                #region 针对道具
                                case BattleCmd.PropsInstruction:
                                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                        PlayerToPropslnstruction(battleTransferDTOs, roleId);
                                    break;
                                #endregion
                                #region 针对技能
                                case BattleCmd.SkillInstruction:
                                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                        PlayerToRelease(battleTransferDTOs, roleId);
                                    break;
                                #endregion
                                #region 针对逃跑
                                case BattleCmd.RunAwayInstruction:
                                    isRunAway = true;
                                    PlayerToRunAway(battleTransferDTOs, roleId);
                                    break;
                                #endregion
                                #region 针对法宝
                                case BattleCmd.MagicWeapon:
                                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                        PlayerToMagicWeapen(battleTransferDTOs,roleId);
                                    break;
                                #endregion
                                case BattleCmd.CatchPet:
                                    break;
                                case BattleCmd.SummonPet:
                                    break;
                                case BattleCmd.Tactical:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "PetStatusDTO":
                            var petStatusDTO = objectOwner as PetStatusDTO;
                            //Utility.Debug.LogInfo("老陆 测试" + _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP);
                            switch (battleTransferDTOs.petBattleTransferDTO.BattleCmd)
                            {
                                case BattleCmd.PropsInstruction:
                                    if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0)
                                        PlayerToPropslnstruction(battleTransferDTOs.petBattleTransferDTO, roleId, battleTransferDTOs.petBattleTransferDTO.RoleId);
                                    break;
                                case BattleCmd.SkillInstruction:
                                    if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0)
                                        PlayerToRelease(battleTransferDTOs.petBattleTransferDTO, roleId, 0, battleTransferDTOs.petBattleTransferDTO.RoleId);
                                    break;
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
                if (_roomidToBattleTransfer.ContainsKey(roomId))
                    _roomidToBattleTransfer[roomId].Add(battleTransferDTOs);
                if (!GameManager.CustomeModule<ServerBattleManager>().RecordTeamRooomId.Contains(roomId))
                {
                    if (!_teamIdToRoomId.ContainsKey(IsTeamDto(roleId).TeamId))
                        _teamIdToRoomId.Add(IsTeamDto(roleId).TeamId, roomId);
                    GameManager.CustomeModule<ServerBattleManager>().RecordTeamRooomId.Enqueue(roomId);
                }
                Utility.Debug.LogInfo("老陆 ，开始战斗的时候收集客户端一个请求"+ _roomidToBattleTransfer[roomId].Count);
            }

        }
        /// 战斗结束
        /// </summary>
        public void BattleEnd(int RoomId)
        {
            var tempRoleId = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit.FirstOrDefault(t => t.Value.RoomId == RoomId).Key;
            var roleStatusSever = _teamIdToBattleInit[tempRoleId].playerUnits[0].RoleStatusDTO;
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
                BattleStart(roleId, roomId, battleTransferDTOs); ;
        }

        private SkillReactionCmd GetSendSkillReactionCmd(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SendSkillReactionCmd;
        }
        private int GetSkillReactionValue(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SkillReactionValue;
        }

    }
}
