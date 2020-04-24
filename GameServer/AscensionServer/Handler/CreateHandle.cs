using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;

namespace AscensionServer.Handler
{
    class CreateHandle:BaseHandler
    {
        public CreateHandle()
        {
            opCode = AscensionProtocol.OperationCode.CreateRole;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            //取得所有已经登录的（在线玩家）的用户名
            List<string> usernaemList = new List<string>();
            foreach (MyClientPeer temPeer in AscensionServer.ServerInstance.peerList)
            {
                if (string.IsNullOrEmpty(temPeer.User.Username) == false && temPeer != peer)
                {
                    usernaemList.Add(temPeer.User.Username);
                }

            }

            //string rolename = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Rolename) as string;
            //string gender = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.Gender) as string;
           // UserManager manager = new UserManager();
           // RoleManager roleManager = new RoleManager();
            //User user = manager.GetByUsername(username);//根据username查询数据

            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
         
            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);

        }

      
    }
}
