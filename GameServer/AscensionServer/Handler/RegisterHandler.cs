﻿/*
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
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var userJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User) as string;
            var userObj = Utility.ToObject<User>(userJson);
            bool isExist = Singleton<NHManager>.Instance.Verify<User>(new NHCriteria() { PropertyName = "Account", Value = userObj.Account });
            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
            if (!isExist)
            {
                //添加输入的用户和密码进数据库
                userObj=  Singleton<NHManager>.Instance.Add(userObj);
                bool userRoleExist = Singleton<NHManager>.Instance.Verify<UserRole>(new NHCriteria() { PropertyName = "UUID", Value = userObj.UUID });
                if (!userRoleExist)
                {
                    var userRole = new UserRole() { UUID = userObj.UUID };
                    Singleton<NHManager>.Instance.Add(userRole);
                }
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
