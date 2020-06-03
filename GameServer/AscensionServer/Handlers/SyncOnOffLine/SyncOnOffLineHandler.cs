using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public class SyncOnOffLineHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.SyncOnOffLine;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncOnOffLineSubHandler>();
        }
    }
}
