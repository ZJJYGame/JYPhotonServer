using AscensionProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public class SyncOnOffLineHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncOnOffLine; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncOnOffLineSubHandler>();
        }
    }
}
