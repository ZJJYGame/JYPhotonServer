using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //处理登陆请求的类
    class LoginHandler : BaseHandler
    {
        public LoginHandler()
        {
            opCode = OperationCode.Login;//赋值OperationCode为Login,表示处理的是那个请求
        }
        //登陆请求的处理的代码
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            //根据发送过来的请求获得用户名和密码
            string username = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;

            string password = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Password) as string;

            UserManager manager = new UserManager();
            bool isSuccess = manager.VerifyUser(username, password);
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            //如果验证成功，把成功的结果利用response.ReturnCode返回成功给客户端
            if (isSuccess)
            {
                response.ReturnCode = (short)ReturnCode.Success;
                peer.username = username;  //保持当前用户的用户名让ClientPeer管理起来
                peer.uuid = manager.GetUUid(username);
            }
            else//否则返回失败给客户端
            {
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(response, sendParameters);

        }
    }
}
