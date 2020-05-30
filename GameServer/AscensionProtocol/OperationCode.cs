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
        SyncOtherRoles=7,
        SyncRoleStatus=8,
        SyncRoleAssets=9,
        SyncInventory=10,
        SyncTask=11,
        SyncGameDate=12,
        SyncMiShu=13,
        SyncGongFa=14,
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
