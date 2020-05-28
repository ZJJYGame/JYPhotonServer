using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
namespace AscensionServer
{
    public class CultivateMiShuHandler : BaseHandler
    {
        public CultivateMiShuHandler()
        {
            OpCode = OperationCode.CultivateMiShu;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.MiShuExp));
            OpResponse.OperationCode = operationRequest.OperationCode;
            ResponseData.Clear();
            if (receivedData != null)
            {
                OpResponse .ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
