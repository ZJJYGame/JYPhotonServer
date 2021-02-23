using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using System;
namespace AscensionServer
{
    public class VerifySutrasAtticmHandler : SyncSutrasAtticmSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Verify;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            return operationResponse;
        }
    }
}


