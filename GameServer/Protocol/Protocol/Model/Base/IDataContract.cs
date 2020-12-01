﻿using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Union(0, typeof(C2SInput))]
    [Union(1, typeof(C2SPlayer))]
    [Union(3, typeof(C2SContainer))]
    [Union(4, typeof(S2CInput))]
    [Union(5, typeof(S2CContainer))]
    [Union(6, typeof(DataParameters))]
    [Union(7, typeof(S2CPlayer))]
    [Union(8, typeof(C2SSkillInput))]
    [Union(9, typeof(C2STransformInput))]
    [Union(10, typeof(C2SFlyMagicToolInput))]
    [Union(11, typeof(C2SAnimateInput))]
    [Union(12, typeof(C2SMapResource))]
    [Union(13, typeof(S2CMapResource))]
    public interface IDataContract { }
}
