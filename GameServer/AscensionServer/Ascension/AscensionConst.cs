using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public partial class AscensionConst
    {
        /// <summary>
        /// 线程间隔时间为毫秒，时间间距： s*1000=ms
        /// 同步登录玩家的位置信息时间间隔
        /// 同步资源刷新的时间间隔
        /// </summary>
        public const int SyncLoggedRolesPositionInterval = 500;

        public const int SyncRoleMoveStasus = 1000;

        public const int SyncResourceInterval = 300000;
    }
}
