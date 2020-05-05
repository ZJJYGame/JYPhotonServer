/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 注册
*/
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    public class RegisterHandler : BaseHandler
    {
        public RegisterHandler()
        {
            opCode = AscensionProtocol.OperationCode.Register;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            string username = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Username) as string;
            string password = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Password) as string;

            User user = Singleton<UserManager>.Instance.GetByUsername(username);//根据username查询数据
            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
            //如果没有查询到代表这个用户没被注册过可用
            if (user == null)
            {
                //添加输入的用户和密码进数据库
                user = new User() { Account = username, Password = password };
                Singleton<UserManager>.Instance.Add(user);
                responser.ReturnCode = (short)ReturnCode.Success;//返回成功

            }
            else//否者这个用户被注册了
            {
                responser.ReturnCode = (short)ReturnCode.Fail;//返回失败
            }
            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);
        }
    }
}
