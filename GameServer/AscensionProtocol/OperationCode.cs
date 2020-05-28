using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
<<<<<<< Updated upstream
        Login=0,
        Logoff=1,
        Register=2,
        SelectRole=3,
        CreateRole=4,
        VerifyRole=5,
        UpdateRole=6,
        RemoveRole=7,
        SyncPositon=8,
        SyncSelfRoles=9,
        SyncOtherRoles=10,
        SyncRoleStatus=11,
        SyncRoleAssets=12,
        Inventory=13,
        Default=14,
        DestroyOtherRole=15,
        HeartBeat=16,
        DistributeTask=17,
        SyncRoleExp=18,
        SyncGameDate=19,
=======
        Login,
        Logoff,
        Register,
        SelectRole,
        CreateRole,
        VerifyRole,
        UpdateRole,
        RemoveRole,
        SyncPositon,
        SyncSelfRoles,
        SyncOtherRoles,
        SyncRoleStatus,
        SyncRoleAssets,
        Inventory,
        Default,
        DestroyOtherRole,
        HeartBeat,
        DistributeTask,
>>>>>>> Stashed changes
        SubCode=128,
    }
}
