using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
    /// <summary>
    /// 历练、秘境通用的子码；
    /// <see cref="AdventureOpCode">
    /// <see cref="SecretAreaOpCode">
    /// </summary>
    public enum LevelCommonOpCode:byte
    {
        Enter=0,
        Exit=1,
        CmdInput=2
    }
}
