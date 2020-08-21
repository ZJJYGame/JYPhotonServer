﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class ImmortalsAllianceDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int  AllianceID { get; set; }
        public virtual byte AllianceJob { get; set; }
        public virtual int Reputation { get; set; }


        public override void Clear()
        {
            RoleID = -1;
            AllianceID = 0;
            AllianceJob = 0;
            Reputation = 0; 
        }
    }
}
