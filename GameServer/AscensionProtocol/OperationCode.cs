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
        Register,
        SelectRole,
        CreateRole,
        Inventory,
        Default
    }
}
