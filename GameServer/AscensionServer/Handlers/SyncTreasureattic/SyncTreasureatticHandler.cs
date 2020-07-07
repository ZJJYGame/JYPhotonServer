using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
    public class SyncTreasureatticHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncTreasureattic;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTreasureatticSubHandler>();

        }
    }
}
