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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
           

        }
    }
}
