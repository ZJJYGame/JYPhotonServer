using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncMiShuHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.SyncMiShu;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncMiShuSubHandler>();
        }
    }
}
