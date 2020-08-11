using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncTemporaryRingHandler: Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.SyncTemInventory;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTemporarySubHandler>();
        }
    }
}
