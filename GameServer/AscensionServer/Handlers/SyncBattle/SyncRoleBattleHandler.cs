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
    public class SyncRoleBattleHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncBattle; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncBattleSubHandler>();
        }
    }
}


