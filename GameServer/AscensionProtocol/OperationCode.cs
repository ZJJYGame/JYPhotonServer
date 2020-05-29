using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
        Default =0,
        Login = 1,
        Logoff=2,
        Register=3,
        SyncRole=4,
        SyncPositon=5,
        SyncSelfRoles=6,
        SyncOtherRoles=10,
        SyncRoleStatus=11,
        SyncRoleAssets=12,
        SyncInventory=13,
        SyncDistributeTask=17,
        SyncGameDate=19,
        SyncMiShu=20,
        SyncGongFa=21,

        /// <summary>
        /// 心跳
        /// </summary>
        HeartBeat = 244,
        /// <summary>
        /// 子操作码
        /// </summary>
        SubOperationCode = 255
    }
}
