using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using Cosmos;
namespace AscensionServer
{
    public interface IHandler:IBehaviour
    {
        OperationCode OpCode { get;  }
        void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
    }
}
