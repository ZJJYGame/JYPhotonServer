using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Union(0, typeof(C2SInput))]
    [Union(1, typeof(C2SPlayer))]
    [Union(3, typeof(C2SEntityContainer))]
    [Union(4, typeof(S2CInput))]
    [Union(5, typeof(S2CEntityContainer))]
    [Union(6, typeof(DataParameters))]
    [Union(7, typeof(S2CPlayer))]
    [Union(8, typeof(C2SSkillInput))]
    [Union(9, typeof(C2STransformInput))]
    [Union(10, typeof(C2SFlyMagicToolInput))]
    public interface IDataContract { }
}
