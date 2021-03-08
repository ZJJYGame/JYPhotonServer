using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol

{
    public enum LoginAreaOpCode : byte
    {
        GetAccountRoles=0,
        CreateRole=1,
        LoginRole=2,
        LogoffRole=3
    }
}
