using AscensionProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncInventoryHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncInventory; } }

        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncInventorySubHandler>();
        }
    }
}
