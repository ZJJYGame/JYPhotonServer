﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
   public class SyncAllianceConstructionHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncAllianceConstruction;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncAllianceConstructionSubHandler>();
        }
    }
}