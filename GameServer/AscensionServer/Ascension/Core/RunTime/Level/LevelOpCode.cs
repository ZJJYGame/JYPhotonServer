using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public enum LevelOpCode:byte
    {
        PlayerSYN = 1,
        PlayerEnter = 2,
        PlayerExit = 3,
        PlayerInput = 4,
        PlayerFIN = 5,
    }
}
