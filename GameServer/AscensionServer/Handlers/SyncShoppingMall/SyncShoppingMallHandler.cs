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
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncShoppingMall;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncShoppingMallSubHandler>();
        }
    }
}
