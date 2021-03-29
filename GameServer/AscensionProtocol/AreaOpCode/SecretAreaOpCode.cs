using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{ 
    public enum SecretAreaOpCode : byte
    {
        Enter = 0,
        Exit = 1,
        CmdInput = 2
    }
}
