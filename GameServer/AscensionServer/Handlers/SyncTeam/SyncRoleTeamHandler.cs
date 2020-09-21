using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class SyncRoleTeamHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleTeam; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTeamSubHandler>();
        }
    }
}
