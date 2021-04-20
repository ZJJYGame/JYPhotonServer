using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public enum MultiplayResOperationCode:byte
    {
        None=0,
        SYN=1,
        Collect=2,
        Battle=3,
        FIN=4
    }
}
