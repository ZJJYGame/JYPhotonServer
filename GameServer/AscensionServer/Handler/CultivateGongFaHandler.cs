using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
namespace AscensionServer
{
    public class CultivateGongFaHandler : BaseHandler
    {
        public CultivateGongFaHandler()
        {
            opCode = OperationCode.CultivateGongFa;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.GongFaExp));
            OpResponse.OperationCode = operationRequest.OperationCode;
            ResponseData.Clear();
            if (receivedData!=null)
            {
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
