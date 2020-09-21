using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public class GetTreasureatticHandler : SyncTreasureatticSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
           
        }
    }
}