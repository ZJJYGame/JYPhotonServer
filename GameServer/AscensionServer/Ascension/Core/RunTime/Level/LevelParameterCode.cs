using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public enum LevelParameterCode:byte
    {
        Existed = 1,
        EnteredRole = 2,
        ExitedRole = 3,
        LevelType=4,
        InputData=5,
        ServerSyncInterval=6
    }
}
