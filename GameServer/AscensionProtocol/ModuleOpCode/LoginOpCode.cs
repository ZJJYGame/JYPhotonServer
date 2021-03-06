using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol

{
    public enum LoginOpCode:byte
    {
        GetAccountRoles=0,
        CreateRole=1
    }
}
