using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public partial class ApplicationBuilder
    {
        /// <summary>
        /// 地图资源刷新周期；
        /// 以秒计算；
        /// </summary>
        public const int MapResourceRefreshInterval = 300000;
        /// <summary>
        /// 服务器帧数；
        /// 换算成毫秒1000/tick;
        /// </summary>
        public const int TICKRATE = 10;
        /// <summary>
        /// 每个tick所持续的毫秒；
        /// </summary>
        public static readonly int _MSPerTick;
        static ApplicationBuilder()
        {
            _MSPerTick = 1000 / TICKRATE;
        }
    }
}
