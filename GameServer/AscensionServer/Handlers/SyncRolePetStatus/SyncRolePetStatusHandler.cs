using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using FluentNHibernate.Testing.Values;
using NHibernate.Mapping;
namespace AscensionServer
{
   public class SyncRolePetStatusHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRolePet;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRolePetStatusSubHandler>();
        }
    }
}
