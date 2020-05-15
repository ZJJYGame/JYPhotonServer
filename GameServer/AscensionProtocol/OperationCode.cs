using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    public enum OperationCode:byte//区分请求和响应
    {
        Login,
        Logoff,
        Register,
        SelectRole,
        CreateRole,
        SyncPositon,
        SyncPlayer,
        SyncRoleStatus,
        VerifyRoleStatus,
        Verify,
        Inventory,
        Default,
        Destroy,
    }
}
