﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using UnityEngine;
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
            if (_oldBattleList.Count > 0 )
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
                //battleInit.battleUnits = new AllBattleDataDTO() { }
                _teamIdToBattleInit.Add(battleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, battleInit);
                _roomidToBattleTransfer.Add(battleInit.RoomId, new List<BattleTransferDTO>());
            }
        }


        /// <summary>
        /// 准备指令战斗 
        /// </summary>
        public void PrepareBattle(int roleId)
        {
            if (IsTeamDto(roleId) == null)
                return;
            else
            {
                //TODO  缺少 针对组队
                teamIdList.Add(roleId);
                for (int i = 0; i < teamIdList.Count; i++)
                {
                    if (teamIdList[i] == IsTeamDto(roleId).TeamMembers[i].RoleID)
                    {

                    }
                }
                teamIdList.Clear();
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
            ReleaseToSpeed();
            if (!_roomidToBattleTransfer.ContainsKey(roomId))
                return;

            if (IsTeamDto(roleId) == null)
            {
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillMiShuDatas>>(out var skillMiShuDict);
                //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var monsterDict);

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
                        {
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
                                            tempTrans.isFinish = false;
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
                                    tempTransEnemy.isFinish = false;
                                    tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p];

                                    PlayerInfosSet.Add(tempTransEnemy);
                                    teamSet.Add(new BattleTransferDTO() { BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].enemyUnits[l].EnemyStatusDTO.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });

                                }
                            }

                        }//多段攻击
                        else if(skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.Staged)
                        {
                            /*
                            for (int k = 0; k < TargetID.Count; k++)
                            {
                                if (_teamIdToBattleInit[roleId].enemyUnits[k].GlobalId == TargetID[k])
                                {
                                    for (int o = 0; o < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; o++)
                                    {
                                        //需要判断 当前血量是不是满足条件
                                        _teamIdToBattleInit[roleId].enemyUnits[k].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o];
                                        //ProcessDamageSet.Add(skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o]);
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = TargetID[k];
                                        tempTrans.TargetHPDamage = skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[o];
                                        TargetInfosSet.Add(tempTrans);
                                        teamSet.Add(new BattleTransferDTO() { RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                            }*/
                        }
                    }
                    else if (skillMiShuDict.ContainsKey(battleTransferDTOs.TargetInfos[i].TargetID))
                    {

                    }
                }
               

                switch (battleTransferDTOs.SendSkillReactionCmd)
                {
                    case SkillReactionCmd.BeatBack:
                        break;
                    case SkillReactionCmd.Guard:
                        break;
                    case SkillReactionCmd.Dodge:
                        break;
                    case SkillReactionCmd.Shock:
                        break;
                    case SkillReactionCmd.Parry:
                        break;
                }
            }
            else
            {
                //teamSet.Add(battleTransferDTOs);
                //for (int i = 0; i < teamSet.Count; i++)
                //{
                //    switch (GetSendSkillReactionCmd(roomId,i))
                //    {
                //        case BattleTransferDTO.SkillReactionCmd.BeatBack:
                //            break;
                //        case BattleTransferDTO.SkillReactionCmd.Guard:
                //            break;
                //        case BattleTransferDTO.SkillReactionCmd.Dodge:
                //            break;
                //        case BattleTransferDTO.SkillReactionCmd.Shock:
                //            break;
                //        case BattleTransferDTO.SkillReactionCmd.Parry:
                //            break;
                //    }
                //}
            }
                //_roomidToBattleTransfer[roomId].Add(teamSet[i]);
            //teamSet.Clear();
        }


        private SkillReactionCmd GetSendSkillReactionCmd(int roomId, int i)
        {
            return  _roomidToBattleTransfer[roomId][i].SendSkillReactionCmd;
        }
        private int GetSkillReactionValue(int roomId, int i)
        {
            return _roomidToBattleTransfer[roomId][i].SkillReactionValue;
        }


        /// <summary>
        /// 处理每回合
        /// </summary>
        public void isFinishMethod()
        {

        }
        /// <summary>
        /// 战斗结束
        /// </summary>
        public void BattleEnd()
        {

        }

        public int TotalTime = 15;

        public bool isFinish = false;
        /// <summary>
        /// 每回合 倒计时
        /// </summary>
        public override void OnRefresh()
        {
            //var now =  Utility.Time.SecondNow();
            if (isFinish)
            {
                TotalTime--;
                if (TotalTime == 0)
                {
                    TotalTime = 15;
                    isFinish = false;
                }
            }

        }

    }
}
