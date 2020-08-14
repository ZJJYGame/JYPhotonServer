using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;

namespace AscensionServer
{
   public class SyncPetaPtitudeHandler: Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncPetaPtitude;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncPetaPtitudeSubHandler>();
        }
    }
}
