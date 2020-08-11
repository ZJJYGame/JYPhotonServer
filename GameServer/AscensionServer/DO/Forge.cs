﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Forge : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual int JobLevel { get; set; }
        public virtual int JobLevelExp { get; set; }
        public virtual string Recipe_Array { get; set; }

        public Forge()
        {
            JobLevel = 4;
            Recipe_Array = null;
            JobLevelExp = 0;
        }
        public override void Clear()
        {
            RoleID = -1;
            JobLevel = 0;
            JobLevelExp = 0;
            Recipe_Array = null;
        }
    }
}