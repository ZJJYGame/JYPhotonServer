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

namespace AscensionServer
{
    /// <summary>
    /// 定时器类
    /// </summary>
    public class Timer
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

        public Timer(float second)
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
}
