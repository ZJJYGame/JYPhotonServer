﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class FlyMagicTool: DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual string  AllFlyMagicTool { get; set; }
        public virtual string FlyToolLayoutDict { get; set; }
        public FlyMagicTool()
        {
            AllFlyMagicTool = null;
            FlyToolLayoutDict = null;
        }

        public override void Clear()
        {
            RoleID = -1;
            AllFlyMagicTool=null;
            FlyToolLayoutDict = null;
        }
    }
}
