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
        public override byte OpCode { get { return (byte)OperationCode.SyncHerbsField; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncHerbsFieldSubHandler>();
        }
    }
}


