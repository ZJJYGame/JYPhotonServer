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
                battleInit.countDownSec = battleInitDTO.countDownSec;
                battleInit.roundCount = battleInitDTO.roundCount;
                battleInit.playerUnits = RoleInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.petUnits = battleInitDTO.petUnits;//PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.enemyUnits = EnemyInfo(battleInitDTO.enemyUnits);
                battleInit.enemyPetUnits = battleInitDTO.enemyPetUnits;// PetInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID);
                battleInit.maxRoundCount = battleInitDTO.maxRoundCount;
                battleInit.battleUnits = AllBattleDataDTOsInfo(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInitDTO);
                _teamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());
                _roomidToTimer.Add(battleInit.RoomId, new TimerToManager(RoleBattleTime));
                
            }
        }


        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        public void PrepareBattle(int roleId)
        {

            if (IsTeamDto(roleId) == null)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = roleId + "=>个人服务器 组队 准备完成， over！";
                //TODO 展示使用这个
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
                GameManager.CustomeModule<ServerBattleManager>().RecordTeamId.Enqueue(IsTeamDto(roleId).TeamId);
                _teamidToTimer.Add(IsTeamDto(roleId).TeamId, new TimerToManager(10000));
                List<int> memberSet = new List<int>();
                memberSet.Add(roleId);
                _teamIdToMemberDict.Add(IsTeamDto(roleId).TeamId, memberSet);
                GameManager.CustomeModule<ServerBattleManager>().TimestampBattlePrepare(IsTeamDto(roleId).TeamId);

            }
        }


        /// <summary>
        /// 开始战斗   -->  开始战斗的回合
        /// </summary>
        public void BattleStart(int roleId, int roomId, BattleTransferDTO battleTransferDTOs)
        {
            TargetID.Clear();
            teamSet.Clear();
            PlayerInfosSet.Clear();
            TargetInfosSet.Clear();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillMiShuDatas>>(out var skillMiShuDict);
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
                    switch (typeName)
                    {
                        case "EnemyStatusDTO":
                            var enemyStatusData = objectOwner as EnemyStatusDTO;
                            if (enemyStatusData.EnemyHP > 0&& _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                AIToRelease(battleTransferDTOs, enemyStatusData, roleId, skillGongFaDict);
                            break;
                        case "RoleStatusDTO":
                            if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
                                PlayerToRelease(battleTransferDTOs, roleId, skillGongFaDict);
                            break;
                    }
                }
                OperationData opData = new OperationData();
                opData.DataMessage = RoundServerToClient();
                opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
                GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
                GameManager.CustomeModule<ServerBattleManager>().RecordRoomId.Enqueue(roomId);
                GameManager.CustomeModule<ServerBattleManager>().TimestampBattleEnd(roomId);

                #region ob

                /*

                for (int i = 0; i < battleTransferDTOs.TargetInfos.Count; i++)
                {
                    if (skillGongFaDict.ContainsKey(battleTransferDTOs.ClientCmdId))
                    {
                        Utility.Debug.LogInfo("老陆真长++>" + battleTransferDTOs.TargetInfos[i].TargetID);
                        TargetID.Add(battleTransferDTOs.TargetInfos[i].TargetID, battleTransferDTOs.TargetInfos[i].GlobalId);
                        while (TargetID.Count != skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number)
                        {
                            if (TargetID.Count == skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number)
                                break;
                            //TODO 缺少判断  是不是死亡

                            var index = new Random().Next(0, _teamIdToBattleInit[roleId].enemyUnits.Count);
                            if (TargetID.ContainsKey(_teamIdToBattleInit[roleId].enemyUnits[index].EnemyStatusDTO.EnemyId))
                                continue;
                            TargetID.Add(_teamIdToBattleInit[roleId].enemyUnits[index].EnemyStatusDTO.EnemyId, _teamIdToBattleInit[roleId].enemyUnits[index].GlobalId);
                        }

                        //一次性攻击
                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type  == AttackProcess_Type.SingleUse)
                        {0
                            for (int p = 0; p < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; p++)
                            {
                                for (int k = 0; k < TargetID.Count; k++)
                                {
                                    TargetInfosSet.Clear();
                                 
                                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                                        {
                                            //需要判断 当前血量是不是满足条件
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p];
                                            //ProcessDamageSet.Add(skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p]);
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p];

                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }
                                    }
                                }

                                for (int l = 0; l < _teamIdToBattleInit[roleId].enemyUnits.Count; l++)
                                {
                                    PlayerInfosSet.Clear();

                                    _teamIdToBattleInit[roleId].playerUnits[p].RoleStatusDTO.RoleHP -= _teamIdToBattleInit[roleId].enemyUnits[l].EnemyStatusDTO.EnemyAttact_Power;
                                    BattleTransferDTO.TargetInfoDTO tempTransEnemy = new BattleTransferDTO.TargetInfoDTO();
                                    tempTransEnemy.TargetID = roleId;
                                    tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p];

                                    PlayerInfosSet.Add(tempTransEnemy);
                                    teamSet.Add(new BattleTransferDTO() { BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].enemyUnits[l].EnemyStatusDTO.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                                }
                            }

                        }//多段攻击
                        else if(skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.Staged)
                        {

                            for (int k = 0; k < TargetID.Count; k++)
                            {
                                for (int b = 0; b < _teamIdToBattleInit[roleId].enemyUnits.Count; b++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[b].EnemyStatusDTO.EnemyId == TargetID[k])
                                    {
                                        for (int o = 0; o < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; o++)
                                        {
                                            //需要判断 当前血量是不是满足条件
                                            _teamIdToBattleInit[roleId].enemyUnits[b].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o];
                                            //ProcessDamageSet.Add(skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o]);
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            tempTrans.TargetID = TargetID[k];
                                            tempTrans.TargetHPDamage = skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o];
                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { BattleCmd = RoleDTO.BattleCmd.SkillInstruction,  RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else if (skillMiShuDict.ContainsKey(battleTransferDTOs.TargetInfos[i].TargetID))
                    {

                    }
                }*/
                #endregion
            }
            else
            {
                if (_roomidToBattleTransfer.ContainsKey(roomId))
                    _roomidToBattleTransfer[roomId].Add(battleTransferDTOs);
                if (!GameManager.CustomeModule<ServerBattleManager>().RecordTeamRooomId.Contains(roomId))
                {
                    _teamIdToRoomId.Add(IsTeamDto(roleId).TeamId, roomId);
                    GameManager.CustomeModule<ServerBattleManager>().RecordTeamRooomId.Enqueue(roomId);
                }
                
                Utility.Debug.LogInfo("老陆 ，开始战斗的时候收集客户端一个请求"+ _teamIdToRoomId.Count);
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




        private SkillReactionCmd GetSendSkillReactionCmd(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SendSkillReactionCmd;
        }
        private int GetSkillReactionValue(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SkillReactionValue;
        }

        /*
        /// <summary>
        /// 每回合 倒计时
        /// </summary>
        int updateInterval = ApplicationBuilder._MSPerTick;
        long latestTime;
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var now = Utility.Time.MillisecondTimeStamp();
            if (now >= latestTime)
            {
                //广播当前帧，并进入下一帧；
                latestTime = now + updateInterval;

                Utility.Debug.LogWarning("=>老陆_currentTime OnRefresh");
            }
        }*/

    }
}
