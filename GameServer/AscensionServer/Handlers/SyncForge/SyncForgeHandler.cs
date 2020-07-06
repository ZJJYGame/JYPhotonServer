using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
   public  class SyncForgeHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncForge;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncForgeSubHandler>();
        }
    }
}
