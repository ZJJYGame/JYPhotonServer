using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer.Handler
{
    public class SyncRoleExpHandler : BaseHandler
    {
        public SyncRoleExpHandler()
        {
            OpCode = OperationCode.SyncRoleExp;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleid = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleID));
        }
    }
}
