using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public  class SyncShoppingMallHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncShoppingMall; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncShoppingMallSubHandler>();
        }
    }
}
