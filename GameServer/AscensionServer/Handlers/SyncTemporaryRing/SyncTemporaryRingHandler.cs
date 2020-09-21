using AscensionProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncTemporaryRingHandler: Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncTemInventory; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTemporarySubHandler>();
        }
    }
}
