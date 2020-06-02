using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
namespace AscensionServer
{
    public interface ISubHandler:IBehaviour
    {
        SubOperationCode SubOpCode { get;  }
        Handler Owner { get; set; }
        void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
    }
}
