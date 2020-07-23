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
using System;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 处理登陆请求的类
    /// </summary>
   public class LoginHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.Login;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            //根据发送过来的请求获得用户名和密码
            string userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            NHCriteria nHCriteriaAccount = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            NHCriteria nHCriteriaPassword = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Password", userObj.Password);
            bool verified = ConcurrentSingleton<NHManager>.Instance.Verify<User>(nHCriteriaAccount, nHCriteriaPassword);
            ResponseData.Clear();
            //如果验证成功，把成功的结果利用response.ReturnCode返回成功给客户端
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (verified)
            {
               OpResponse.ReturnCode = (short)ReturnCode.Success;
                userObj.UUID= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<User>(nHCriteriaAccount).UUID;
                peer.Login(userObj);
                AscensionServer.Instance.AddIntoLoggedUserCache(peer);
                AscensionServer._Log.Info("~~~~~~~~~~~~~~~~~~~~~~Login Success : " + userObj.Account + " ; UUID : " + peer.PeerCache.UUID + "~~~~~~~~~~~~~~~~~~~~~~");
                ResponseData.Add((byte)ParameterCode.Role, Utility.Json.ToJson(userObj));
                OpResponse.Parameters = ResponseData;
            }
            else
            {
                AscensionServer._Log.Info("Login fail:" + userObj.Account);
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAccount, nHCriteriaPassword);
        }
    }
}
