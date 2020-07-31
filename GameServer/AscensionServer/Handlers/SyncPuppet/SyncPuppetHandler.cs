using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
    public  class SyncPuppetHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncPuppet;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncPuppetSubHandler>();
        }
    }
}
