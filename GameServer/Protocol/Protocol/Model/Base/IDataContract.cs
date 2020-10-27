using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Union(0, typeof(C2SInput))]
    [Union(1, typeof(C2SPlayer))]
    [Union(2, typeof(FixVector3))]
    [Union(4, typeof(C2SEntityContainer))]
    [Union(5, typeof(S2CInput))]
    [Union(6, typeof(S2CEntityContainer))]
    [Union(7, typeof(DataParameters))]
    [Union(8, typeof(S2CPlayer))]
    [Union(9, typeof(FixSkill))]
    public interface IDataContract
    {
    }
}
