﻿using System;
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

        #region 出手速度 以及出手拥有者 以及对Ai的血量判断
        /// <summary>
        /// 出手速度
        /// </summary>
        public void ReleaseToSpeed(int roleId)
        {
            if (_teamIdToBattleInit.ContainsKey(roleId))
                _teamIdToBattleInit[roleId].battleUnits = _teamIdToBattleInit[roleId].battleUnits.OrderByDescending(t => t.ObjectSpeed).ToList();

            foreach (var item in _teamIdToBattleInit[roleId].battleUnits)
            {
                Utility.Debug.LogInfo("老陆 ，出手速度" + item.ObjectSpeed + "<>" + item.ObjectName);
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
            if (_teamIdToBattleInit[roleId].petUnits.Find(t => (t.ObjectId == objectId)) != null)
                return _teamIdToBattleInit[roleId].petUnits.Find(t => (t.ObjectId == objectId)).PetStatusDTO;
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
                    tempDataSet.Add(_teamIdToBattleInit[roleId].enemyUnits[i]);
            }
            return tempDataSet;
        }

        /// <summary>
        /// 针对玩家  血量 >0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleBattleDatas"></param>
        /// <returns></returns>
        public List<RoleBattleDataDTO> PlayerToHPMethod(int roleId,int currentRoleId,List<RoleBattleDataDTO> roleBattleDatas)
        {
            List<RoleBattleDataDTO> tempDataSet = new List<RoleBattleDataDTO>();
            for (int i = 0; i < roleBattleDatas.Count; i++)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[i].RoleStatusDTO.RoleID == currentRoleId)
                    continue;
                if (_teamIdToBattleInit[roleId].playerUnits[i].RoleStatusDTO.RoleHP > 0)
                    tempDataSet.Add(_teamIdToBattleInit[roleId].playerUnits[i]);
            }
            return tempDataSet;
        }
        /// <summary>
        /// 针对宠物 血量>0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="petBattleDataDTOs"></param>
        /// <returns></returns>
        public List<PetBattleDataDTO> PetToHPMethod(int roleId, List<PetBattleDataDTO> petBattleDataDTOs)
        {
            List<PetBattleDataDTO> tempDataSet = new List<PetBattleDataDTO>();
            for (int i = 0; i < petBattleDataDTOs.Count; i++)
                if (_teamIdToBattleInit[roleId].petUnits[i].PetStatusDTO.PetHP > 0)
                    tempDataSet.Add(_teamIdToBattleInit[roleId].petUnits[i]);
            return tempDataSet;
        }


        #endregion

        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        public void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId,int  transfer = 0)
        {
            BattleTransferDTO.TargetInfoDTO tempTransEnemy = new BattleTransferDTO.TargetInfoDTO();
            //Utility.Debug.LogInfo("<enemyStatusData  老陆>" + _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP);
            if ((IsTeamDto(roleId) == null))
            {
                ///TODO  需要怪物的技能表格 释放技能
                if (_teamIdToBattleInit[roleId].petUnits.Count !=0&& _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP>0)
                {
                    var RandomTarget = new Random().Next(0, 2);
                    var target = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= 100 : _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP -= 100;
                    tempTransEnemy.TargetID = RandomTarget == 0 ? roleId : _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetID;
                    tempTransEnemy.TargetHPDamage = -100; //-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
                else
                {
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= 100;
                    tempTransEnemy.TargetID = roleId;
                    tempTransEnemy.TargetHPDamage = -100; //-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
               
            }
            else
            {
                //Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                var petObject = _teamIdToBattleInit[roleId].petUnits.Find(x => x.PetStatusDTO.PetHP > 0);
                if (_teamIdToBattleInit[roleId].petUnits.Count != 0 && petObject != null)
                {
                    var RandomTarget = new Random().Next(0, 2);
                    var target = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= 100 : petObject.PetStatusDTO.PetHP -= 100;
                    tempTransEnemy.TargetID = RandomTarget == 0 ? _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID : petObject.PetStatusDTO.PetID;
                    tempTransEnemy.TargetHPDamage = -100;
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
                else
                {
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= 100;
                    tempTransEnemy.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTransEnemy.TargetHPDamage = -100;//-skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                    List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    PlayerInfosSet.Add(tempTransEnemy);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
                }
            }
        }



        #region   单人 技能指令使用功法的技能
        /// <summary>
        /// 针对功法  筛选出来每回合的存活的Al
        /// </summary>
        public void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, SkillGongFaDatas skillGongFa)
        {
            var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
            int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
            TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
            while (TargetID.Count != skillGongFa.Attack_Number)
            {
                if (TargetID.Count == survivalNumber || survivalNumber == 0)
                    break;
                //TODO 缺少判断   数量不够 的情况下
                indexNumber++;
                //var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == indexNumber - 1)
                    break;
                if (TargetID.ContainsKey(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId))
                    continue;

                TargetID.Add(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].GlobalId);
            }
        }
        /// <summary>
        /// 针对功法  玩家释放  不同技能类型的技能计算伤害
        /// </summary>
        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs,int roleId, SkillGongFaDatas skillGongFa,int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillGongFa.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillGongFa.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion
                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        //Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillGongFa.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 针对宠物的技能计算
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        /// <param name="special"></param>
        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId,int petId, SkillGongFaDatas skillGongFa, int special = 0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);

                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillGongFa.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillGongFa.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion
                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        //Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillGongFa.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 技能类型治疗术   TODO 存在多个目标时加血的目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId, SkillGongFaDatas skillGongFa)
        {
            ///给自己回血
            if (skillGongFa.Attack_Number == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
               BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = roleId;
                tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

            }///多目标回血   ??? TODO  缺少多个数量
            else if (skillGongFa.Attack_Number >1)
            {

            }
        }
        #endregion

        #region 单人技能指令使用秘术的技能
        public void AlToSurvival(BattleTransferDTO battleTransferDTOs, int roleId, int info, SkillMiShuDatas skillMiShu)
        {
            var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
            int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
            TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
            while (TargetID.Count != skillMiShu.Attack_Number)
            {
                if (TargetID.Count == survivalNumber || survivalNumber == 0)
                    break;
                //TODO 缺少判断   数量不够 的情况下
                indexNumber++;
                //var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == indexNumber - 1)
                    break;
                if (TargetID.ContainsKey(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId))
                    continue;

                TargetID.Add(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].EnemyStatusDTO.EnemyId, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber - 1].GlobalId);
            }
        }

        public void PlayerToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, SkillMiShuDatas skillMiShu)
        {
            ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
            ///单人 单段和多段伤害
            if (skillMiShu.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            ///换取目标打
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }

                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillMiShu.Attack_Number > 1)
            {
                //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                //var allEnemyHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                ///TargetId 需要对目标出手的数量
                ///TargetID
                for (int k = 0; k < skillMiShu.Attack_Factor.Count; k++)
                {
                    if (skillMiShu.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillMiShu.Attack_Factor.Count != 1)
                        {
                            #region ob  TODO

                            /*
                                for (int ko = 0; ko < skillMiShu.Attack_Factor.Count; ko++)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        break;
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[ko];

                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[ko];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    if (skillMiShu.Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    else
                                        teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                }*/
                            #endregion

                        }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }

                    }
                    else if (skillMiShu.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        Utility.Debug.LogInfo("单人多个数量攻击伤害" + TargetID.Count);
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillMiShu.Attack_Factor.Count != 1)
                            {

                            }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }

                }
            }
        }
        #endregion

        #region 单人技能分类具体处理

        /// <summary>
        /// 针对单人的功法秘术   处理玩家  判断Al出手之前 是不是死亡 换取目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        public void PlayerToRelease(BattleTransferDTO battleTransferDTOs, int roleId,int special = 0,int petId = 0)
        {
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (IsToSkillForm(battleTransferDTOs.ClientCmdId))
                {
                    //Utility.Debug.LogInfo("survivalNumber================ =>" + survivalNumber);
                    //TODO  需要细节的处理
                    var objectOwner = SkillFormToSkillObject(battleTransferDTOs.ClientCmdId);
                    var typeName =  objectOwner.GetType().Name;
                    switch (typeName)
                    {
                        case "SkillGongFaDatas":
                            var skillGongFa = objectOwner as SkillGongFaDatas;
                            switch ((Skill_Type)skillGongFa.Skill_Type-1)
                            {
                                case Skill_Type.Attact:
                                    AlToSurvival(battleTransferDTOs, roleId, info, skillGongFa);
                                    if (petId == 0)
                                        PlayerToSkillDamage(battleTransferDTOs, roleId, skillGongFa, special);
                                    else
                                        PlayerToSkillDamage(battleTransferDTOs, roleId, petId, skillGongFa, special);
                                    break;
                                case Skill_Type.ReturnBlood:
                                    PlayerToSkillReturnBlood(battleTransferDTOs, roleId, skillGongFa);
                                    break;
                                case Skill_Type.Shield:
                                    break;
                                case Skill_Type.Buffer:
                                    break;
                                case Skill_Type.Resurgence:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "SkillMiShuDatas":
                            var skillMiShu = objectOwner as SkillMiShuDatas;
                            AlToSurvival(battleTransferDTOs, roleId, info, skillMiShu);
                            PlayerToSkillDamage(battleTransferDTOs, roleId, skillMiShu);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 针对法宝的使用
        /// </summary>
        public void PlayerToMagicWeapen(BattleTransferDTO battleTransferDTOs, int roleId)
        {
            if (MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var magicOwner =  MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId);
            if (!IsToSkillForm(magicOwner.Magic_Skill))
                return;
            //Utility.Debug.LogInfo(" battleTransferDTOs.ClientCmdId ===》法宝指令" + battleTransferDTOs.ClientCmdId);
            battleTransferDTOs.ClientCmdId = magicOwner.Magic_Skill;
            PlayerToRelease(battleTransferDTOs, roleId, magicOwner.Magic_ID);
        }

       /// <summary>
       /// 针对  单人逃跑的 返回计算处理
       /// </summary>
        public void PlayerToRunAway(BattleTransferDTO battleTransferDTOs, int roleId)
        {
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
            tempTrans.TargetID = roleId;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }

        /// <summary>
        /// 针对  单人的道具的使用 返回计算处理 符箓和丹药
        /// </summary>
        public void PlayerToPropslnstruction(BattleTransferDTO battleTransferDTOs,int roleId,int petId = 0)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataToUser(battleTransferDTOs, roleId, drugData,petId);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataToUser(battleTransferDTOs,roleId, runesData,petId);
                    break;
            }
        }

        /// <summary>
        /// 丹药的使用
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="drugData"></param>
        public void DrugDataToUser(BattleTransferDTO battleTransferDTOs, int roleId,DrugData drugData,int petId = 0)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugHP(battleTransferDTOs,roleId,drugData,petId);
                    break;
                case DrugType.RoleMP:

                    break;  
                case DrugType.RoleBuff:

                    break;

                case DrugType.RoleResurgence:
                    break;
            }
        }
        #region 单人 针对丹药的HP  MP Buffer 复活
        public void DrugHP(BattleTransferDTO battleTransferDTOs, int roleId, DrugData drugData,int petId = 0)
        {
            if (drugData.Drug_Use_Target == 1) // 需要加宠物
            {
                if (petId == 0)
                {
                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = roleId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP)
                        _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP;
                    else
                        _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = petId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
            else
            {
                if (petId == 0)
                {
                    if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = roleId;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    TargetInfosSet.Add(tempTrans);
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    for (int ov = 0; ov < 2; ov++)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP > 0&& ov == 0)
                        {
                            if (_teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP)
                                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleMaxHP;
                            else
                                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = roleId;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                            TargetInfosSet.Add(tempTrans);
                        }
                        if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP > 0 && ov ==1)
                        {
                            if (_teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP)
                                _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP = _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetMaxHP;
                            else
                                _teamIdToBattleInit[roleId].petUnits[0].PetStatusDTO.PetHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = petId;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                           
                            TargetInfosSet.Add(tempTrans);
                           
                        }
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = petId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }
        #endregion


        /// <summary>
        /// 符箓的使用
        /// </summary>
        public void RunesDataToUser(BattleTransferDTO battleTransferDTOs, int roleId,RunesData runesData,int petId =0)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            PlayerToRelease(battleTransferDTOs, roleId, runesData.Runes_ID, petId);
        }

        #endregion





        /// <summary>
        /// 针对 技能组队 计算  and 玩家出手
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentRole"></param>
        /// <param name="transfer"></param>
        /// 如果special = -1 的时候代表有宠物
        public void PlayerTeamToRelease(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole,int transfer = 0,int special = 0)
        {
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.ClientCmdId);
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (IsToSkillForm(battleTransferDTOs.ClientCmdId))
                {
                    var objectOwner = SkillFormToSkillObject(battleTransferDTOs.ClientCmdId);
                    var typeName = objectOwner.GetType().Name;
                    switch (typeName)
                    {
                        case "SkillGongFaDatas":
                            var skillGongFa = objectOwner as SkillGongFaDatas;
                            switch ((Skill_Type)skillGongFa.Skill_Type - 1)
                            {
                                case Skill_Type.Attact:
                                    AlToSurvival(battleTransferDTOs, roleId, info, skillGongFa);
                                    PlayerTeamToSkillDamage(battleTransferDTOs, roleId, currentRole, skillGongFa, special);
                                    break;
                                case Skill_Type.ReturnBlood:
                                    PlayerTeamToSkillReturnBlood(battleTransferDTOs, roleId, currentRole, skillGongFa, transfer);
                                    break;
                                case Skill_Type.Shield:
                                    break;
                                case Skill_Type.Buffer:
                                    break;
                                case Skill_Type.Resurgence:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "SkillMiShuDatas":
                            var skillMiShu = objectOwner as SkillMiShuDatas;
                            AlToSurvival(battleTransferDTOs, roleId, info, skillMiShu);
                            PlayerTeamToSkillDamage(battleTransferDTOs, roleId, currentRole, skillMiShu);
                            break;
                    }
                }
            }
        }

        #region 组队技能指令使用功法的技能
        /// <summary>
        /// 玩家组队技能伤害计算
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="currentRole"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerTeamToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, SkillGongFaDatas skillGongFa,int special =0)
        {
            battleTransferDTOs.ClientCmdId = battleTransferDTOs.BattleCmd == BattleCmd.PropsInstruction || battleTransferDTOs.BattleCmd == BattleCmd.MagicWeapon ? special : battleTransferDTOs.ClientCmdId;
            ///一段伤害
            if (skillGongFa.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            //TODO
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    //ProcessDamageSet.Add(skillGongFa.Attack_Factor[p]);
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillGongFa.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillGongFa.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillGongFa.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillGongFa.Attack_Number > 1)
            {
                for (int k = 0; k < skillGongFa.Attack_Factor.Count; k++)
                {
                    if (skillGongFa.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillGongFa.Attack_Factor.Count != 1) { }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                    else if (skillGongFa.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillGongFa.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFa.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillGongFa.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }

        public void PlayerTeamToSkillDamage(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, SkillMiShuDatas skillMiShu)
        {
            ///一段伤害
            if (skillMiShu.Attack_Number == 1)
            {
                for (int k = 0; k < TargetID.Count; k++)
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                    for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                    {
                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                        {
                            //TODO
                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                            {
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {
                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];

                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                                else
                                {
                                    //需要判断 当前血量是不是满足条件
                                    _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                    //ProcessDamageSet.Add(skillMiShu.Attack_Factor[p]);
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                            else
                            {
                                ///TODO  应该不完整
                                if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == 0)
                                {
                                    Utility.Debug.LogError("AI  全部死亡");
                                    //BattleEnd()
                                    return;
                                }
                                ///判断技能的伤害系数是一个还是多个
                                if (skillMiShu.Attack_Factor.Count != 1)
                                {

                                    for (int op = 0; op < skillMiShu.Attack_Factor.Count; op++)
                                    {
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            break;
                                        var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[op];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[op];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (skillMiShu.Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }

                                }
                                else
                                {
                                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                    var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                    tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                    tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    TargetInfosSet.Add(tempTrans);
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                            }
                        }
                    }
                }
            }
            ///多段伤害
            else if (skillMiShu.Attack_Number > 1)
            {
                for (int k = 0; k < skillMiShu.Attack_Factor.Count; k++)
                {
                    if (skillMiShu.AttackProcess_Type == AttackProcess_Type.SingleUse)
                    {
                        ///判断技能伤害系数是一个还是多个
                        if (skillMiShu.Attack_Factor.Count != 1) { }
                        else
                        {
                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                            for (int op = 0; op < TargetID.Count; op++)
                            {
                                var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = TargetID.Keys.ToList()[op];
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                TargetInfosSet.Add(tempTrans);
                            }
                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                        }
                    }
                    else if (skillMiShu.AttackProcess_Type == AttackProcess_Type.Staged)
                    {
                        for (int n = 0; n < TargetID.Count; n++)
                        {
                            var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                            ///判断技能伤害系数是一个还是多个
                            if (skillMiShu.Attack_Factor.Count != 1) { }
                            else
                            {
                                survivalTarget.EnemyStatusDTO.EnemyHP -= skillMiShu.Attack_Factor[0];
                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                tempTrans.TargetHPDamage = -skillMiShu.Attack_Factor[0];
                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                TargetInfosSet.Add(tempTrans);
                                if (TargetID.Count - 1 == n)
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                else
                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 组队回血
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFa"></param>
        public void PlayerTeamToSkillReturnBlood(BattleTransferDTO battleTransferDTOs, int roleId,int currentRole, SkillGongFaDatas skillGongFa,int transfer)
        {
            if (skillGongFa.Attack_Number == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];

                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                if (skillGongFa.Attack_Number -1 > PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count)
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                    TargetInfosSet.Add(tempTrans);
                    for (int ol = 0; ol < PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count; ol++)
                    {
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[ol].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0]>= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                    tempTrans.TargetHPDamage = skillGongFa.Attack_Factor[0];
                    TargetInfosSet.Add(tempTrans);
                    for (int ol = 0; ol < skillGongFa.Attack_Number -1; ol++)
                    {
                        var IndexOl = new Random().Next(0, PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count);
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[IndexOl].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + skillGongFa.Attack_Factor[0] >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += skillGongFa.Attack_Factor[0];
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = skillGongFa.Attack_Factor[0];
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }


        /// <summary>
        /// 组队逃跑   可以和并成一个 和单人逃跑的   需要去队伍中标记一下 是不是存在战斗中还是中途退出啦
        /// 需要继续完善
        /// </summary>
        public void PlayerTeamToRunAway(BattleTransferDTO battleTransferDTOs,int roleId,int currentRole,int transfer = 0,int speed =0)
        {
            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
            tempTrans.TargetID = currentRole;
            tempTrans.TargetHPDamage = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP > 0 ? 1 : 0;
            if (tempTrans.TargetHPDamage == 1)
            {
                isRunAway++;
                //_teamIdToBattleInit[roleId].playerUnits.RemoveAt(transfer);
                //_teamIdToBattleInit[roleId].battleUnits.RemoveAt(speed);
            }
            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
            TargetInfosSet.Add(tempTrans);
            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
        }

        /// <summary>
        /// 针对 组队的法宝使用
        /// </summary>
        public void PlayerTeamToMagicWeapon(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole,int transfer)
        {
            if (MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var magicOwner = MagicWeaponFormToObject(battleTransferDTOs.ClientCmdId);
            if (!IsToSkillForm(magicOwner.Magic_Skill))
                return;
            battleTransferDTOs.ClientCmdId = magicOwner.Magic_Skill;
            PlayerTeamToRelease(battleTransferDTOs, roleId, currentRole, transfer, magicOwner.Magic_ID);
        }

        /// <summary>
        /// 针对 组队道具 符箓和丹药
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        public void PlayerTeamToPropslnstruction(BattleTransferDTO battleTransferDTOs, int roleId,int currentRole,int transfer)
        {
            if (PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId) == null)
                return;
            var objectOwner = PropsInstrutionFormToObject(battleTransferDTOs.ClientCmdId);
            var typeName = objectOwner.GetType().Name;
            switch (typeName)
            {
                case "DrugData":
                    var drugData = objectOwner as DrugData;
                    DrugDataTeamToUser(battleTransferDTOs, roleId,currentRole, transfer, drugData);
                    break;
                case "RunesData":
                    var runesData = objectOwner as RunesData;
                    RunesDataTeamToUser(battleTransferDTOs, roleId, currentRole, runesData, transfer);
                    break;
            }
        }

        /// <summary>
        /// 针对组队的符箓的使用
        /// </summary>
        public void RunesDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole, RunesData runesData, int transfer)
        {
            if (!IsToSkillForm(runesData.Runes_Skill))
                return;
            battleTransferDTOs.ClientCmdId = runesData.Runes_Skill;
            PlayerTeamToRelease(battleTransferDTOs, roleId,  currentRole, transfer, runesData.Runes_ID);
        }


        /// <summary>
        /// 针对组队的丹药的使用
        /// </summary>
        public void DrugDataTeamToUser(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole,int transfer, DrugData drugData)
        {
            switch (drugData.Drug_Type)
            {
                case DrugType.RoleHP:
                    DrugTeamHP(battleTransferDTOs, roleId,currentRole, transfer, drugData);
                    break;
                case DrugType.RoleMP:

                    break;
                case DrugType.RoleBuff:

                    break;

                case DrugType.RoleResurgence:
                    break;
            }
        }
        /// <summary>
        /// 针对丹药的HP  回血    //缺少宠物的判断
        /// </summary>
        public void DrugTeamHP(BattleTransferDTO battleTransferDTOs, int roleId, int currentRole,int transfer, DrugData drugData)
        {
            if (drugData.Drug_Use_Target == 1)
            {
                if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                else
                    _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                tempTrans.TargetID = currentRole;
                tempTrans.TargetHPDamage = drugData.Drug_Value;
                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                TargetInfosSet.Add(tempTrans);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
            }
            else
            {
                if (drugData.Drug_Use_Target >= PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count)
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    for (int ot = 0; ot < _teamIdToBattleInit[roleId].playerUnits.Count; ot++)
                    {
                        if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP > 0)
                        {
                            if (_teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP)
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleMaxHP;
                            else
                                _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                            tempTrans.TargetID = _teamIdToBattleInit[roleId].playerUnits[ot].RoleStatusDTO.RoleID;
                            tempTrans.TargetHPDamage = drugData.Drug_Value;
                            TargetInfosSet.Add(tempTrans);
                        }
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
                else
                {
                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();

                    if (_teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP + drugData.Drug_Value >= _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP)
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleMaxHP;
                    else
                        _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP += drugData.Drug_Value;
                    BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                    tempTrans.TargetID = currentRole;
                    tempTrans.TargetHPDamage = drugData.Drug_Value;
                    TargetInfosSet.Add(tempTrans);
                    for (int oc = 0; oc < drugData.Drug_Use_Target-1; oc++)
                    {
                        //PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count
                        var IndexOl = new Random().Next(0, PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits).Count);
                        var tempRoleObject = _teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == PlayerToHPMethod(roleId, currentRole, _teamIdToBattleInit[roleId].playerUnits)[IndexOl].RoleStatusDTO.RoleID);
                        if (tempRoleObject.RoleStatusDTO.RoleHP + drugData.Drug_Value >= tempRoleObject.RoleStatusDTO.RoleMaxHP)
                            tempRoleObject.RoleStatusDTO.RoleHP = tempRoleObject.RoleStatusDTO.RoleMaxHP;
                        else
                            tempRoleObject.RoleStatusDTO.RoleHP += drugData.Drug_Value;
                        BattleTransferDTO.TargetInfoDTO tempTransOl = new BattleTransferDTO.TargetInfoDTO();
                        tempTransOl.TargetID = tempRoleObject.RoleStatusDTO.RoleID;
                        tempTransOl.TargetHPDamage = drugData.Drug_Value;
                        TargetInfosSet.Add(tempTransOl);
                    }
                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = battleTransferDTOs.BattleCmd, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                }
            }
        }

        #endregion

    }
}
