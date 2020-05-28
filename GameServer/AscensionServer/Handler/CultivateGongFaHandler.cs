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
        }
    }
}
