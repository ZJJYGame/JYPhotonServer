using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using System.Diagnostics;

namespace AscensionServer
{
   public class SyncRoleAllianceHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleAlliance; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleAllianceSubHandler>();
        }

    }
}


