using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using System.Collections.Generic;
using System;
using Cosmos;

namespace AscensionServer
{
    public class SyncBottleneckHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncBottleneck; } }

        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncBottleneckSubHandler>();
        }
    }
}
