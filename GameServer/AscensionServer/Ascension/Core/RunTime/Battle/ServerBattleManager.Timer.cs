﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;
using System.Security.Cryptography;
using System.Timers;
using Protocol;

namespace AscensionServer
{
    /// <summary>
    /// 定时器类
    /// </summary>
    public class TimerManager
    {
        /// <summary>
        /// 是否在计时中
        /// </summary>
        bool _isTicking;
        /// <summary>
        /// 当前时间
        /// </summary>
        public float _currentTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public float _endTime;

        public delegate void EvnentHander();

        public TimerManager(float second)
        {
            _currentTime = 0;
            _endTime = second;
        }
        /// <summary>
        /// 开始计时
        /// </summary>
        public void StartTimer()
        {
            _isTicking = true;
        }
        /// <summary>
        /// 更新中
        /// </summary>
        /// <param name="delteTime"></param>
        public void UpdateTimer(float delteTime)
        {
            if (_isTicking)
            {
                _currentTime += delteTime;
                Utility.Debug.LogInfo(_currentTime + " =>_currentTime");
                if (_currentTime > _endTime)
                {
                    _isTicking = false;
                }
            }
        }
        /// <summary>
        /// 停止计时
        /// </summary>
        public void StopTimer()
        {
            _isTicking = false;
        }

        /// <summary>
        /// 持续计时
        /// </summary>
        public void ContinueTimer()
        {
            _isTicking = true;
        }

        /// <summary>
        /// 重新计时
        /// </summary>
        public void ReStartTimer()
        {
            _isTicking = true;
            _currentTime = 0;
        }
        /// <summary>
        /// 重新设定计时器
        /// </summary>
        /// <param name="second"></param>
        public void ResetEndTimer(float second)
        {
            _endTime = second;
        }

    }


    #region 正在使用的倒计时

    public class TimerToManager
    {
        public BattleEndDelegateHandle EndDelegateHandle;

        public BattlePrepareDelegateHandle PrepareDelegateHandle;

        public BattleStartDelegateHandle startDelegateHandle;

        System.Timers.Timer Mytimer;

        public TimerToManager(int second)
        {
            Mytimer = new System.Timers.Timer(second);
        }

        #region 这个时间用于每回合战斗的倒计时

        /// <summary>
        /// 应对 每回合战斗的开始时间
        /// </summary>
        public void StartTimer()
        {
            Mytimer.Enabled = true;
            Mytimer.Start();
            Mytimer.Elapsed += new ElapsedEventHandler(BattleMethodCallBack);
            Mytimer.AutoReset = false;
        }
        /// <summary>
        /// 绑定的回调事件
        /// </summary>
        public void BattleMethodCallBack(object sender, ElapsedEventArgs args)
        {
            EndDelegateHandle += BattleEndCallBackMethod;
            EndDelegateHandle?.Invoke();
        }

        /// <summary>
        /// 停止时间
        /// </summary>
        public void BattleEndStopTimer()
        {
            EndDelegateHandle -= BattleEndCallBackMethod;
            Mytimer.Stop();
        }

        /// <summary>
        /// 处理回调
        /// </summary>
        public void BattleEndCallBackMethod()
        {
            Utility.Debug.LogInfo("老陆 , 每回合倒计时结束 ！");
            int teamp = GameManager.CustomeModule<ServerBattleManager>().RecordRoomId.Dequeue();
            GameManager.CustomeModule<ServerBattleManager>().BattleIsDie(teamp);
            BattleEndStopTimer();
        }
        #endregion

        #region 这个时间用于准备阶段的倒计时
        /// <summary>
        /// ，初始化 加载完毕  开始时间
        /// </summary>
        public void PrepareTimer()
        {
            Mytimer.Enabled = true;
            Mytimer.Start();
            Mytimer.Elapsed += new ElapsedEventHandler(PrepareBattleMethodCallBack);
            Mytimer.AutoReset = false;
        }

        public void PrepareBattleMethodCallBack(object sender, ElapsedEventArgs args)
        {
            PrepareDelegateHandle += BattlePrepareCallBackMethod;
            PrepareDelegateHandle?.Invoke();
        }

        public void BattlePrepareCallBackMethod()
        {
            Utility.Debug.LogInfo("老陆 ，初始化 加载完毕 准备开始");

            var tempTeamId = GameManager.CustomeModule<ServerBattleManager>().RecordTeamId.Dequeue();
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
            BattlePrepareStopTimer();
            GameManager.CustomeModule<ServerBattleManager>()._teamidToTimer.Add(tempTeamId, new TimerToManager(10000));
            GameManager.CustomeModule<ServerBattleManager>().TimestampBattleStart(tempTeamId);
        }
        /// <summary>
        /// 准备 停止时间
        /// </summary>
        public void BattlePrepareStopTimer()
        {
            EndDelegateHandle -= BattleEndCallBackMethod;
            Mytimer.Stop();
        }

        #endregion

        #region 倒计时开始战斗回调

        public void BattleStartTimer()
        {
            Mytimer.Enabled = true;
            Mytimer.Start();
            Mytimer.Elapsed += new ElapsedEventHandler(StartBattleMethodCallBack);
            Mytimer.AutoReset = false;
        }

        public void StartBattleMethodCallBack(object sender, ElapsedEventArgs args)
        {
            startDelegateHandle += BattleStartCallBackMethod;
            startDelegateHandle?.Invoke();
        }

        public void BattleStartCallBackMethod()
        {
            Utility.Debug.LogInfo("老陆 ，战斗开始结算");
            bool isHangUp = false;
            var serverBattleManager = GameManager.CustomeModule<ServerBattleManager>();
            var serverTeamManager = GameManager.CustomeModule<ServerTeamManager>();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            ///房间id
            var teampRoomId = serverBattleManager.RecordTeamRooomId.Dequeue();
            ///相当于队长
            var tempRole = serverBattleManager._teamIdToBattleInit.ToList().Find(x => x.Value.RoomId == teampRoomId);
            ///队伍id
            var tempTeamId = serverBattleManager.IsTeamDto(tempRole.Key).TeamId;

            //Utility.Debug.LogInfo("老陆 ，战斗开始结算111111111111=>房间id" + teampRoomId);
            //Utility.Debug.LogInfo("老陆 ，战斗开始结算111111111111=>房间id" + tempRole.Key);
            //Utility.Debug.LogInfo("老陆 ，战斗开始结算111111111111=>队长id" + tempTeamId);
            Utility.Debug.LogInfo("老陆 ，战斗开始结算111111111111=>长度" + serverBattleManager._teamIdToMemberDict[tempTeamId].Count);
            if (GameManager.CustomeModule<ServerBattleManager>()._teamIdToMemberDict.ContainsKey(tempTeamId))
            {
                ///先判断队伍中食是不是所有队员都发消息
                if (serverBattleManager._roomidToBattleTransfer[teampRoomId].Count != serverTeamManager._teamTOModel[tempTeamId].TeamMembers.Count)
                {
                    isHangUp = true;
                    for (int i = 0; i < serverTeamManager._teamTOModel[tempTeamId].TeamMembers.Count; i++)
                    {
                        //Utility.Debug.LogInfo("老陆 ，战斗开始结算222222222222==>" + serverTeamManager._teamTOModel[tempTeamId].TeamMembers[i]);
                        serverBattleManager._teamIdToMemberDict[tempTeamId].Add(serverTeamManager._teamTOModel[tempTeamId].TeamMembers[i].RoleID);
                        //TODO  ////默认是用第一个战斗传输的数据
                        BattleTransferDTO battleTransfer = new BattleTransferDTO();
                        battleTransfer.RoleId = serverTeamManager._teamTOModel[tempTeamId].TeamMembers[i].RoleID;
                        battleTransfer.isFinish = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].isFinish;
                        battleTransfer.BattleCmd = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].BattleCmd;
                        battleTransfer.ClientCmdId = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].ClientCmdId;
                        battleTransfer.TargetInfos = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].TargetInfos;
                        battleTransfer.SkillReactionValue = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].SkillReactionValue;
                        battleTransfer.SendSkillReactionCmd = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].SendSkillReactionCmd;
                        battleTransfer.RoleIdShieldValueDict = serverBattleManager._roomidToBattleTransfer[teampRoomId][0].RoleIdShieldValueDict;
                        if (serverBattleManager._roomidToBattleTransfer[teampRoomId].Find(x => x.RoleId == serverTeamManager._teamTOModel[tempTeamId].TeamMembers[i].RoleID) != null)
                            continue;
                        serverBattleManager._roomidToBattleTransfer[teampRoomId].Add(battleTransfer);
                    }
                }
                ///排列一下出手速度
                serverBattleManager.ReleaseToSpeed(tempRole.Key);
                ///TODO  需要判断技能是不是多段伤害
                //int transfer = 0;

                for (int speed = 0; speed < serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits.Count; speed++)
                {
                    var objectOwner = serverBattleManager.ReleaseToOwner(serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits[speed].ObjectID, serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits[speed].ObjectId, tempRole.Key);
                    var typeName = objectOwner.GetType().Name;

                    switch (typeName)
                    {
                        case "EnemyStatusDTO":
                            //Utility.Debug.LogInfo("老陆 ，EnemyStatusDTOEnemyStatusDTO");
                            var enemyStatusData = objectOwner as EnemyStatusDTO;
                            ///返回一个当前要出手的人的个人属性
                            var EnemyIndex = new Random().Next(0, serverBattleManager._roomidToBattleTransfer[teampRoomId].Count);
                            var memberCuuentTranferEnemy = serverBattleManager._teamIdToBattleInit[tempRole.Key].playerUnits.Find(x => x.RoleStatusDTO.RoleID == serverBattleManager._roomidToBattleTransfer[teampRoomId][EnemyIndex].RoleId);
                            if (enemyStatusData.EnemyHP > 0 && memberCuuentTranferEnemy.RoleStatusDTO.RoleHP > 0)
                            {
                                serverBattleManager.AIToRelease(serverBattleManager._roomidToBattleTransfer[teampRoomId][EnemyIndex], enemyStatusData, tempRole.Key, skillGongFaDict, EnemyIndex);
                            }
                            break;
                        case "RoleStatusDTO":
                            //Utility.Debug.LogInfo("老陆 ，RoleStatusDTORoleStatusDTO");
                            ///返回一个当前要出手的人的个人选择的传输的战斗数据
                            var speedCuurentTransfer = serverBattleManager._roomidToBattleTransfer[teampRoomId].Find(q => q.RoleId == serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits[speed].ObjectID);
                            ///返回一个当前要出手的人的个人属性
                            var memberCuuentTranfer = serverBattleManager._teamIdToBattleInit[tempRole.Key].playerUnits.Find(x => x.RoleStatusDTO.RoleID == serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits[speed].ObjectID);
                            if (memberCuuentTranfer.RoleStatusDTO.RoleHP > 0)
                                serverBattleManager.PlayerToRelease(speedCuurentTransfer, tempRole.Key, skillGongFaDict, serverBattleManager._teamIdToBattleInit[tempRole.Key].battleUnits[speed].ObjectID);
                            //transfer++;
                            break;
                    }
                }
            }

            ///通知所有玩家当前回合战斗计算完毕
            for (int op = 0; op < serverBattleManager._teamIdToMemberDict[tempTeamId].Count; op++)
            {
                Utility.Debug.LogInfo("发给客户端" + serverBattleManager._teamIdToMemberDict[tempTeamId][op]);
                OperationData opData = new OperationData();
                opData.DataMessage = serverBattleManager.RoundServerToClient();
                opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
                GameManager.CustomeModule<RoleManager>().SendMessage(serverBattleManager._teamIdToMemberDict[tempTeamId][op], opData);
            }

            GameManager.CustomeModule<ServerBattleManager>()._teamidToTimer.Remove(tempTeamId);
            if (isHangUp)
                GameManager.CustomeModule<ServerBattleManager>()._teamIdToMemberDict[tempTeamId] = new List<int>();
            GameManager.CustomeModule<ServerBattleManager>()._roomidToBattleTransfer[teampRoomId] = new List<BattleTransferDTO>();
            GameManager.CustomeModule<ServerBattleManager>().teamSet.Clear();
            BattleStartStopTimer();
            GameManager.CustomeModule<ServerBattleManager>().RecordRoomId.Enqueue(tempTeamId);
            GameManager.CustomeModule<ServerBattleManager>().TimestampBattleEnd(tempTeamId);
        }


        public void BattleStartStopTimer()
        {
            startDelegateHandle -= BattleStartCallBackMethod;
            Mytimer.Stop();
        }

        #endregion



        #endregion
    }

}