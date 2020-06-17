using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer
{
    public class MQHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.MessageQueue;
            base.OnInitialization();
            OnSubHandlerInitialization<MQSubHandler>();
        }
        //public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        //{
        //    AscensionServer._Log.Info("==================MQ==================");
        //    AscensionServer._Log.Info("MQ消息");
        //    AscensionServer._Log.Info("==================MQ==================");
        //    ResponseData.Clear();
        //    OpResponse.Parameters = ResponseData;
        //    OpResponse.ReturnCode = (byte)ReturnCode.Success;
        //    OpResponse.OperationCode = operationRequest.OperationCode;
        //    peer.SendOperationResponse(OpResponse, sendParameters);
        //}
    }
}
