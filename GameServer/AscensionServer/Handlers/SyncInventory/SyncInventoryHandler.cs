using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncInventoryHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.SyncInventory;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncInventorySubHandler>();
        }
    }
}
