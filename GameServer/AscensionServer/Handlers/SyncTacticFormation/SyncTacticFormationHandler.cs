﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
   public  class SyncTacticFormationHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncTacticFormation; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTacticFormationSubHandler>();
        }
    }
}
