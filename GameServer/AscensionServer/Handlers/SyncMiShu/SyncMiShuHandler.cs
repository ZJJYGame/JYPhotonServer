using AscensionProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class SyncMiShuHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncMiShu; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncMiShuSubHandler>();
        }
    }
}
