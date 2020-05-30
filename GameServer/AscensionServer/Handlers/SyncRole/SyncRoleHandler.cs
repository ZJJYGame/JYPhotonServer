using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public class SyncRoleHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRole;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleSubHandler>();
        }
    }
}
