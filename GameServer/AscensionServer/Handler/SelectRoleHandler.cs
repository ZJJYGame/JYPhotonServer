using AscensionProtocol;
using AscensionServer.Model;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Handler
{
    class SelectRoleHandler :BaseHandler
    {
        public SelectRoleHandler()
        {
            opCode = AscensionProtocol.OperationCode.SelectRole;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, ClientPeer peer)
        {
            string username = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Username) as string;
            UserManager manager = new UserManager();
            //RoleManager roleManager = new RoleManager();
            User user = manager.GetByUsername(username);//根据username查询数据

            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
           
            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);
            
        }
    }
}
