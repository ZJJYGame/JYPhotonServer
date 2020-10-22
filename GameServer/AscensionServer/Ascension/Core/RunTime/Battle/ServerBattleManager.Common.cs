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
                if (roleStatusSever.RoleHP <= 0)
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
                teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = PlayerInfosSet });
            }
            else
            {
                Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                //Utility.Debug.LogInfo("老陆 ，=>" + _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID);
                ////TODO 传输玩家的id  可能会出现bug
                //_teamIdToBattleInit[roleId].playerUnits.Find(x => x.RoleStatusDTO.RoleID == battleTransferDTOs.RoleId).RoleStatusDTO.RoleHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                tempTransEnemy.TargetID = _teamIdToBattleInit[roleId].playerUnits[transfer].RoleStatusDTO.RoleID;
                tempTransEnemy.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                List<BattleTransferDTO.TargetInfoDTO> PlayerInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
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
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.RoleId);
            //Utility.Debug.LogInfo("老陆 ，roleId =>" + battleTransferDTOs.ClientCmdId);
            TargetID.Clear();
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
                    ///一段伤害     先判断数量  在判断攻击模式 最后是伤害系数
                    //Utility.Debug.LogInfo("老陆 ，battleTransferDTOs.ClientCmdId=>" + battleTransferDTOs.ClientCmdId);
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
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count !=0)
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
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = _teamIdToBattleInit[roleId].playerUnits[0].RoleStatusDTO.RoleID, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }
                                    }
                                    else
                                    {

                                        ////TODO 需要处理是不是伤害系统 
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
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = roleId, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                    }
                                }
                            }
                        }
                    }
                    ///多段伤害
                    else if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number > 1 )
                    {
                        Utility.Debug.LogInfo("单人多段伤害");

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
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
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
                    //Utility.Debug.LogInfo("老陆 ，battleTransferDTOs.ClientCmdId=>" + battleTransferDTOs.ClientCmdId);
                    if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Number == 1)
                    {
                        for (int k = 0; k < TargetID.Count; k++)
                        {
                            //Utility.Debug.LogInfo("老陆 ，TargetID=>" + TargetID.Count);
                            for (int n = 0; n < _teamIdToBattleInit[roleId].enemyUnits.Count; n++)
                            {
                                if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId == TargetID.ToList()[k].Key)
                                {
                                    if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP > 0)
                                    {
                                        ///判断技能的伤害系数是一个还是多个
                                        if (skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor.Count != 0)
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
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                                else
                                                    teamSet.Add(new BattleTransferDTO() { isFinish = false, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
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
                                            teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });
                                        }
                                        /*
                                        //需要判断 当前血量是不是满足条件
                                        _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP -= skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        if (_teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP <= 0)
                                            _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyHP = 0;
                                        BattleTransferDTO.TargetInfoDTO tempTrans = new BattleTransferDTO.TargetInfoDTO();
                                        tempTrans.TargetID = _teamIdToBattleInit[roleId].enemyUnits[n].EnemyStatusDTO.EnemyId;
                                        tempTrans.TargetHPDamage = -skillGongFaDict[battleTransferDTOs.ClientCmdId].Attack_Factor[0];
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
                                        TargetInfosSet.Add(tempTrans);
                                        teamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = RoleDTO.BattleCmd.SkillInstruction, RoleId = currentRole, ClientCmdId = battleTransferDTOs.ClientCmdId, TargetInfos = TargetInfosSet });*/
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
                                        List<BattleTransferDTO.TargetInfoDTO> TargetInfosSet = new List<BattleTransferDTO.TargetInfoDTO>();
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

                }
            }
        }


    }
}
