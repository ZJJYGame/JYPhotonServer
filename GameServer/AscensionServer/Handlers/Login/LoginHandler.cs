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
using System.ServiceModel.Configuration;

namespace AscensionServer
{
    /// <summary>
    /// 处理登陆请求的类
    /// </summary>
   public class LoginHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.Login; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            //根据发送过来的请求获得用户名和密码
            string userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            NHCriteria nHCriteriaPassword = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Password", userObj.Password);
            bool verified = NHibernateQuerier.Verify<User>(nHCriteriaAccount, nHCriteriaPassword);
            responseParameters.Clear();
            //如果验证成功，把成功的结果利用response.ReturnCode返回成功给客户端
            operationResponse.OperationCode = operationRequest.OperationCode;
            if (verified)
            {
                operationResponse.ReturnCode = (short)ReturnCode.Success;
                userObj.UUID = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaAccount).UUID;
                //peer.Login(userObj);

                //GameManager.External.GetModule<PeerManager>().TryGetValue();
                //var pe = PeerEntity.Create(peer);
                //GameManager.CustomeModule<PeerManager>().TryAdd(pe.SessionId, pe);
                responseParameters.Add((byte)ParameterCode.Role, Utility.Json.ToJson(userObj));
                operationResponse.Parameters = responseParameters;
            }
            else
            {
                Utility.Debug.LogError("Login fail:" + userObj.Account);
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAccount, nHCriteriaPassword);
            return operationResponse;
        }
    }
}
