using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer
{
    public class EnterAdventureSceneHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = AscensionProtocol.OperationCode.EnterAdventureScene;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //var resultJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer.Instance.ExitAdventureScene(peer);
            OpResponse.ReturnCode = (byte)ReturnCode.Success;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
