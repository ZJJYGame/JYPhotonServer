using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
namespace AscensionServer
{
    public class UpdateGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.GongFa));
           Owner. OpResponse.OperationCode = operationRequest.OperationCode;
           Owner. ResponseData.Clear();
            if (receivedData != null)
            {
               Owner. OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                Owner. OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner. OpResponse, sendParameters);
        }
    }
}
