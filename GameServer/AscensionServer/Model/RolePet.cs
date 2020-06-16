﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RolePet:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string PetIDDict { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            PetIDDict = null;
        }
    }
}
