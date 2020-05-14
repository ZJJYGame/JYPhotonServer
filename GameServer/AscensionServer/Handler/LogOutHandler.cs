/*
*Author : Don
*Since 	:2020-05-11
*Description  : 登出处理
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    /// <summary>
    /// 账号登出处理者
    /// </summary>
    public class LogOutHandler : BaseHandler
    {
        public LogOutHandler()
        {
            opCode = OperationCode.Logoff;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string userJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User) as string;
            var userObj = Utility.ToObject<User>(userJson);
            bool verified = Singleton<NHManager>.Instance.Verify<User>(new NHCriteria() { PropertyName = "account", Value = userObj.Account },
                new NHCriteria() { PropertyName = "password", Value = userObj.Password });
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (verified)
            {
                response.ReturnCode =(short) ReturnCode.Success;
                AscensionServer.Instance.DeregisterPeer(peer);//  从已经登录的有序字典中注销
            }
            else
            {
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
