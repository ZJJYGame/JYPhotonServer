using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;


namespace AscensionServer.Handlers
{
   public  class SyncHerbsFieldHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncHerbsField;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncHerbsFieldSubHandler>();
        }
    }
}
