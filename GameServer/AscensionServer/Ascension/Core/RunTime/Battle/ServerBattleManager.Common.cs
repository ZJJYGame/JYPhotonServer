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
        #endregion

        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        public void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId, Dictionary<int, SkillGongFaDatas> skillGongFaDict,int  transfer = 0)
        {
            BattleTransferDTO.TargetInfoDTO tempTransEnemy = new BattleTransferDTO.TargetInfoDTO();
            //Utility.Debug.LogInfo("<enemyStatusData  老陆>" + _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP);
            if ((IsTeamDto(roleId) == null))
            {
                _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0]; // enemyStatusData.EnemyAttact_Power;
                tempTransEnemy.TargetID = roleId;
                tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                PlayerInfosSet.Add(tempTransEnemy);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
            }
            else
            {
                Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                //Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                ////TODO 传输玩家的id  可能会出现bug
                _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                tempTransEnemy.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                PlayerInfosSet.Add(tempTransEnemy);
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
            }
        }

        /// <summary>
        /// 针对单人的   处理玩家  判断Al出手之前 是不是死亡 换取目标
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFaDict"></param>
        public void PlayerToRelease(BattleTransferDTO battleTransferDTOs, int roleId, Dictionary<int, SkillGongFaDatas> skillGongFaDict)
        {
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.RoleId);
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.ClientCmdId);
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (skillGongFaDict.ContainsKey(battleTransferDTOs.ClientCmdId))
                {
                    var indexNumber = 0; //_teamIdToBattleInit[roleId].enemyUnits.Count;
                    int survivalNumber = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count;
                    //Utility.Debug.LogInfo("survivalNumber================ =>" + survivalNumber);
                    //TODO  需要细节的处理
                    TargetID.Add(battleTransferDTOs.TargetInfos[info].TargetID, battleTransferDTOs.TargetInfos[info].GlobalId);
                    while (TargetID.Count != skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number)
                    {
                        if (TargetID.Count == survivalNumber || survivalNumber == 0)
                            break;
                        //TODO 缺少判断   数量不够 的情况下
                        indexNumber ++;
                        //var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                        if (AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count == indexNumber-1)
                            break;
                        if (TargetID.ContainsKey(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber-1].EnemyStatusDTO.EnemyId))
                            continue;
                       
                        TargetID.Add(AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber-1].EnemyStatusDTO.EnemyId, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[indexNumber-1].GlobalId);
                    }
                    //Utility.Debug.LogInfo("TargetID================ =>" + TargetID.Count);

                    ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
                    ///单人 单段和多段伤害
                    if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number == 1)
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
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count !=1)
                                        {
                                            for (int op = 0; op < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; op++)
                                            {
                                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    break;
                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                              
                                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                                tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                                tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                                TargetInfosSet.Add(tempTrans);
                                                if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count-1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                            }
                                        }
                                        else
                                        {
                                            //需要判断 当前血量是不是满足条件
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 1)
                                        {

                                            for (int op = 0; op < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; op++)
                                            {
                                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    break;
                                                var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                                tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                                tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                                TargetInfosSet.Add(tempTrans);
                                                if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                            }

                                        }
                                        else
                                        {
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                            tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }
                                         
                                    }
                                }
                            }
                        }
                    }
                    ///多段伤害
                    else if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number > 1 )
                    {
                        //Utility.Debug.LogInfo("单人多个数量攻击伤害");
                        //var allEnemyHP = GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit[tempRoleId].enemyUnits.Find(q => q.EnemyStatusDTO.EnemyHP > 0);
                        ///TargetId 需要对目标出手的数量
                        ///TargetID
                        for (int k = 0; k < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; k++)
                        {
                            if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.SingleUse)
                            {
                                ///判断技能伤害系数是一个还是多个
                                if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 1)
                                {
                                    #region ob  TODO

                                    /*
                                        for (int ko = 0; ko < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; ko++)
                                        {
                                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                break;
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[ko];

                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[ko];
                                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                            TargetInfosSet.Add(tempTrans);
                                            if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count - 1 == ko || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                            else
                                                teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });

                                        }*/
                                    #endregion

                                }
                                else
                                {
                                    List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                    for (int op = 0; op < TargetID.Count; op++)
                                    {
                                        var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[op]);
                                        survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID =TargetID.Keys.ToList()[op];
                                        tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        TargetInfosSet.Add(tempTrans);
                                    }
                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                }
                                   
                            }
                            else if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.Staged)
                            {
                                Utility.Debug.LogInfo("单人多个数量攻击伤害"+ TargetID.Count);
                                for (int n = 0; n < TargetID.Count; n++)
                                {
                                    var survivalTarget = _teamIdToBattleInit[roleId].enemyUnits.Find(x => x.EnemyStatusDTO.EnemyId == TargetID.Keys.ToList()[n]);
                                    ///判断技能伤害系数是一个还是多个
                                    if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 1)
                                    {
                                      
                                    }
                                    else
                                    {
                                        survivalTarget.EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = survivalTarget.EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        if (TargetID.Count - 1 == n)
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        else
                                            teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                } 
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 针对 组队计算  玩家出手
        /// </summary>
        /// <param name="battleTransferDTOs"></param>
        /// <param name="roleId"></param>
        /// <param name="skillGongFaDict"></param>
        /// <param name="currentRole"></param>
        /// <param name="transfer"></param>
        public void PlayerToRelease(BattleTransferDTO battleTransferDTOs, int roleId, Dictionary<int, SkillGongFaDatas> skillGongFaDict, int currentRole,int transfer = 0)
        {
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.RoleId);
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.ClientCmdId);
            TargetID.Clear();
            ///传输的目标
            for (int info = 0; info < battleTransferDTOs.TargetInfos.Count; info++)
            {
                if (skillGongFaDict.ContainsKey(battleTransferDTOs.ClientCmdId))
                {
                    //Utility.Debug.LogInfo("老陆 ，TargetID=>" + battleTransferDTOs.TargetInfos[info].TargetID);
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
                    if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number == 1)
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
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 1)
                                        {
                                            for (int op = 0; op < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; op++)
                                            {
                                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    break;
                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];

                                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                                tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                                tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                                TargetInfosSet.Add(tempTrans);
                                                if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                            }
                                        }
                                        else
                                        {
                                            //需要判断 当前血量是不是满足条件
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                            //ProcessDamageSet.Add(skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[p]);
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 1)
                                        {

                                            for (int op = 0; op < skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count; op++)
                                            {
                                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    break;
                                                var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);

                                                _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                                tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                                tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[op];
                                                List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                                TargetInfosSet.Add(tempTrans);
                                                if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count - 1 == op || _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                            }

                                        }
                                        else
                                        {
                                            BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                            var index = new Random().Next(0, AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits).Count);
                                            tempTrans.TargetID = AIToHPMethod(roleId, _teamIdToBattleInit[roleId].enemyUnits)[index].EnemyStatusDTO.EnemyId;
                                            tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                            List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                            TargetInfosSet.Add(tempTrans);
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                ///多段伤害
                else if (skillGongFaDict[battleTransferDTOs.ClientCmdId].AttackProcess_Type == AttackProcess_Type.Staged)
                {

                }
            }
        }


    }
}
