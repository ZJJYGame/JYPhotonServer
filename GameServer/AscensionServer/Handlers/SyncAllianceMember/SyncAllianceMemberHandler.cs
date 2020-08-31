﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
  public  class SyncAllianceMemberHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncAllianceMember;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncAllianceMemberSubHandler>();
        }
    }
}