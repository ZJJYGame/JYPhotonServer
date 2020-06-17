using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer {
    public class RemoveMQSubHandler : MQSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //AscensionServer._Log.Info("==================MQ==================");
            //AscensionServer._Log.Info("MQ消息       UpdateMQSubHandler");
            //AscensionServer._Log.Info("==================MQ==================");
            var dict = ParseSubDict(operationRequest);
            SetResponseData(() => {
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
