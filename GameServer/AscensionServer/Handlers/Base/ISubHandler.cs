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
    public interface ISubHandler
    {
        byte SubOpCode { get;  }
        OperationResponse EncodeMessage(OperationRequest operationRequest);
    }
}
