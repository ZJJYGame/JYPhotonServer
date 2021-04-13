using System;
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
            int teamp = GameEntry. ServerBattleManager.RecordRoomId.Dequeue();
            GameEntry.ServerBattleManager.BattleIsDieCallBack(teamp);
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
            var tempTeamId =GameEntry.ServerBattleManager.RecordTeamId.Dequeue();
            GameEntry.ServerBattleManager.BattleTimerPrepareCallBack(tempTeamId);
            BattlePrepareStopTimer();
            GameEntry.ServerBattleManager.TeamidToTimer.Add(tempTeamId, new TimerToManager(10000));
            GameEntry.ServerBattleManager.TimestampBattleStart(tempTeamId);
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
            //bool isHangUp = false;
            var serverBattleManager = GameEntry.ServerBattleManager;
            var serverTeamManager = GameEntry. ServerTeamManager;
            GameEntry. DataManager.TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            ///房间id
            var teampRoomId = serverBattleManager.RecordTeamRooomId.Dequeue();
            ///相当于队长
            var tempRole = serverBattleManager.TeamIdToBattleInit.ToList().Find(x => x.Value.RoomId == teampRoomId);
            ///队伍id
            var tempTeamId = serverBattleManager.IsTeamDto(tempRole.Key).TeamId;

            //Utility.Debug.LogInfo("老陆 ，战斗开始结算111111111111=>房间id" + teampRoomId);
            if (GameEntry. ServerTeamManager.TeamTOModel.ContainsKey(tempTeamId))
            {
                ///先判断队伍中食是不是所有队员都发消息
                if (serverBattleManager.RoomidToBattleTransfer[teampRoomId].Count != serverTeamManager.TeamTOModel[tempTeamId].TeamMembers.Count)
                {
                    //isHangUp = true;
                    serverBattleManager.RoundTeamMember(teampRoomId, tempTeamId,tempRole.Key);
                }
                ///排列一下出手速度
                serverBattleManager.ReleaseToSpeed(tempRole.Key);
                serverBattleManager.RoundTeamSkillComplete(tempRole.Key, teampRoomId, tempTeamId);
            }
            GameEntry.ServerBattleManager.TeamidToTimer.Remove(tempTeamId);
            //if (isHangUp)
            //    GameEntry.ServerBattleManager._teamIdToMemberDict[tempTeamId] = new List<int>();
            GameEntry.ServerBattleManager.RoomidToBattleTransfer[teampRoomId] = new List<BattleTransferDTO>();
            GameEntry.ServerBattleManager.TeamSet.Clear();
            GameEntry.ServerBattleManager.BuffToRoomIdBefore.Clear();
            GameEntry.ServerBattleManager.BuffToRoomIdAfter.Clear();
            BattleStartStopTimer();
            GameEntry.ServerBattleManager.RecordRoomId.Enqueue(teampRoomId);
            GameEntry.ServerBattleManager.TimestampBattleEnd(teampRoomId);
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


