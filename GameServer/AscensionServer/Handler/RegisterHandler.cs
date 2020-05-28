/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 注册
*/
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using System;

namespace AscensionServer
{
    public class RegisterHandler : BaseHandler
    {
        public RegisterHandler()
        {
            OpCode = OperationCode.Register;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User));
            var userObj = Utility.ToObject<User>(userJson);
            NHCriteria nHCriteriaAccount = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            bool isExist = Singleton<NHManager>.Instance.Verify<User>(nHCriteriaAccount);
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (!isExist)
            {
                //添加输入的用户和密码进数据库
                userObj=  Singleton<NHManager>.Instance.Add(userObj);
                NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                bool userRoleExist = Singleton<NHManager>.Instance.Verify<UserRole>(nHCriteriaUUID);
                if (!userRoleExist)
                {
                    var userRole = new UserRole() { UUID = userObj.UUID };
                    Singleton<NHManager>.Instance.Add(userRole);
                }
            OpResponse.ReturnCode = (short)ReturnCode.Success;//返回成功
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID);
            }
            else//否者这个用户被注册了
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;//返回失败
            }
            // 把上面的结果给客户端
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAccount);
        }
    }
}
