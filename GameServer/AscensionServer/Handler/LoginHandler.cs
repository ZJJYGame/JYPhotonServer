/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 登录
*/
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using System.Collections.Generic;
namespace AscensionServer
{
    /// <summary>
    /// 处理登陆请求的类
    /// </summary>
    class LoginHandler : BaseHandler
    {
        public LoginHandler()
        {
            opCode = OperationCode.Login;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //根据发送过来的请求获得用户名和密码
            string userJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User) as string;
            var userObj = Utility.ToObject<User>(userJson);
            AscensionServer.log.Info("userJson>>>>>>>>>>>>>>>>>>>:" + userJson+ "<<<<<<<<<<<<<<<<<<<<");

            bool verified = Singleton<NHManager>.Instance.Verify<User>(new NHCriteria() { PropertyName = "Account", Value = userObj.Account },
               new NHCriteria() { PropertyName = "Password", Value = userObj.Password });

            //如果验证成功，把成功的结果利用response.ReturnCode返回成功给客户端

            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (verified)
            {
                response.ReturnCode = (short)ReturnCode.Success;
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                response.Parameters = data;
                peer.Account = userObj.Account;  //保持当前用户的用户名让ClientPeer管理起来
                peer.UUID = Singleton<NHManager>.Instance.CriteriaGet<User>(new NHCriteria() { PropertyName = "Account", Value = userObj.Account }).UUID;
                AscensionServer.Instance.RegisterPeer(peer);//注册到服务器的有序字典中
                AscensionServer.log.Info("~~~~~~~~~~~~~~~~~~~~~~Login Success : " + userObj.Account+"UUID : "+peer.UUID +"~~~~~~~~~~~~~~~~~~~~~~");
            }
            else
            {
                AscensionServer.log.Info("Login fail:" + userObj.Account);
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
