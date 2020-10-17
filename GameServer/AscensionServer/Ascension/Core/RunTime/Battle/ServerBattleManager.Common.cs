using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Google.Protobuf.WellKnownTypes;
using NHibernate.Linq.Clauses;
using Protocol;

namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        //public TimerManager timer;
        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void Timestamp(int roomId)
        {
            _roomidToTimer[roomId].StartTimer();
        }

        /// <summary>
        /// 出手速度
        /// </summary>
        public void ReleaseToSpeed(int roleId)
        {
            if (_teamIdToBattleInit.ContainsKey(roleId))
                _teamIdToBattleInit[roleId].battleUnits = _teamIdToBattleInit[roleId].battleUnits.OrderByDescending(t => t.ObjectSpeed).ToList();
        }

        /// <summary>
        /// AI 玩家 是否死亡 战斗结束 发起事件
        /// </summary>
        public void BattleIsDie(int roomId)
        {
            var tempRoleId = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit.FirstOrDefault(t => t.Value.RoomId == roomId).Key;
            var roleStatusSever = _teamIdToBattleInit[tempRoleId].playerUnits[0].RoleStatusDTO;

            Utility.Debug.LogInfo("老陆   roleStatusSever"+ roleStatusSever.RoleHP);
            if (roleStatusSever.RoleHP < 0)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = "战斗结束啦， over！";
                //TODO 展示使用这个
                opData.OperationCode = (byte)OperationCode.MessageQueue;
                GameManager.CustomeModule<RoleManager>().SendMessage(tempRoleId, opData);
                BattleEnd(roomId);
            }
            else
            {
                OperationData opData = new OperationData();
                opData.DataMessage = "服务器 倒计时10 秒  over！ ";
                opData.OperationCode = (byte)OperationCode.SyncBattle;
                GameManager.CustomeModule<RoleManager>().SendMessage(tempRoleId, opData);
            }
        }


        /// <summary>
        /// 返回一个出手拥有者  玩家或者AI或者宠物
        /// </summary>
        /// <returns></returns>
        public object ReleaseToOwner(int objectID, int objectId, int roleId)
        {
            //Utility.Debug.LogInfo("<出手速度>" + objectID + "<>" + objectId + "<>" + roleId);
            if (_teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)) != null)
                return _teamIdToBattleInit[roleId].playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)).RoleStatusDTO;
            if (_teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)) != null)
                return _teamIdToBattleInit[roleId].enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)).EnemyStatusDTO;
            return null;
        }

        /// <summary>
        /// 针对AI  血量 >0
        /// </summary>
        public List<EnemyBattleDataDTO> AIToHPMethod(int roleId, List<EnemyBattleDataDTO> enemyBattleDatas)
        {
            List<EnemyBattleDataDTO> tempDataSet = new List<EnemyBattleDataDTO>();
            for (int i = 0; i < enemyBattleDatas.Count; i++)
            {
                if (_teamIdToBattleInit[roleId].enemyUnits[i].EnemyStatusDTO.EnemyHP > 0)
                {
                    tempDataSet.Add(_teamIdToBattleInit[roleId].enemyUnits[i]);
                }
            }
            return tempDataSet;
        }

        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        public void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId, Dictionary<int, SkillGongFaDatas> skillGongFaDict)
        {
            PlayerInfosSet.Clear();
            BattleTransferDTO.TargetInfoDTO tempTransEnemy = new BattleTransferDTO.TargetInfoDTO();
            //Utility.Debug.LogInfo("<enemyStatusData  老陆>" + _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP);

            //if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0)
            {
                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= enemyStatusData.EnemyAttact_Power;
                tempTransEnemy.TargetID = roleId;
                tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                PlayerInfosSet.Add(tempTransEnemy);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
            }

        }

        /// <summary>
        /// 处理玩家  判断Al出手之前 是不是死亡 换取目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFaDict"></param>
        public void PlayerToRelease(BattleTransferDTO battleTransferDTOs, int roleId, Dictionary<int, SkillGongFaDatas> skillGongFaDict)
        {
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (skillGongFaDict.ContainsKey(battleTransferDTOs.ClientCmdId))
                {
                    TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
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
                    ///一段伤害
                    if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        for (int k = 0; k < TargetID.Count; k++)
                        {
                            TargetInfosSet.Clear();

                            for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                            {
                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                                    {
                                        //需要判断 当前血量是不是满足条件
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                        //ProcessDamageSet.Add(skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p]);
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];

                                        TargetInfosSet.Add(tempTrans);
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                    else
                                    {
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                        {
                                            Utility.Debug.LogError("AI  全部死亡");
                                            //BattleEnd()
                                            return;
                                        }
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        TargetInfosSet.Add(tempTrans);
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                            }
                        }
                    }
                }
                ///多段伤害
                else if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.Staged)
                {
                    for (int k = 0; k < TargetID.Count; k++)
                    {
                        TargetInfosSet.Clear();
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
                                    teamSet.Add(new BattleTransferDTO() { BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
