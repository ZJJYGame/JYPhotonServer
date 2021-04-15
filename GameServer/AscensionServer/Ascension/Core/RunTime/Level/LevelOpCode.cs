using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public enum LevelOpCode:byte
    {
        PlayerEnter = 74,
        PlayerExit = 75,
        PlayerInput = 76,
    }
}
