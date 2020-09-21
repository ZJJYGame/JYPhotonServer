using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using Cosmos;
namespace AscensionServer
{
    public interface ISubHandler:IBehaviour
    {
        SubOperationCode SubOpCode { get;  }
        byte SubOpcode { get;  }
        Handler Owner { get; set; }
        void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer);
        object EncodeMessage(object message);
    }
}
