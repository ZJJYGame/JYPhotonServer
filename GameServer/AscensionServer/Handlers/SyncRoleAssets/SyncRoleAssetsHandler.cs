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
    public class SyncRoleAssetsHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleAssets;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleAssetsSubHandler>();
        }
    }
}
