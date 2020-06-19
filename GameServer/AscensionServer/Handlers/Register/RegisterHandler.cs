/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 注册
*/
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using System;
using Cosmos;
namespace AscensionServer
{
    public class RegisterHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.Register;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            NHCriteria nHCriteriaAccount = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            bool isExist = Singleton<NHManager>.Instance.Verify<User>(nHCriteriaAccount);
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (!isExist)
            {
                AscensionServer._Log.Info("==========\n  before add UUID ：" +userJson +"\n"+ userObj.UUID + "\n================");

                //添加输入的用户和密码进数据库
                userObj =  Singleton<NHManager>.Instance.Insert(userObj);
                AscensionServer._Log.Info("==========\n after add UUID ：" + userJson + "\n" + userObj.UUID+"\n================");
                NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                bool userRoleExist = Singleton<NHManager>.Instance.Verify<UserRole>(nHCriteriaUUID);
                if (!userRoleExist)
                {
                    var userRole = new UserRole() { UUID = userObj.UUID };
                    Singleton<NHManager>.Instance.Insert(userRole);
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
