using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    [Union(0, typeof(CmdInput))]
    [Union(1, typeof(SessionRoleIdPair))]
    [Union(3, typeof(C2SContainer))]
    [Union(8, typeof(C2SSkillInput))]
    [Union(10, typeof(C2SFlyMagicToolInput))]
    public interface IDataContract { }
}
