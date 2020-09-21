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
        public override byte OpCode { get { return (byte)OperationCode.Register; } }

        protected override  OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.User));
            var userObj = Utility.Json.ToObject<User>(userJson);
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            bool isExist = NHibernateQuerier.Verify<User>(nHCriteriaAccount);
            ResponseData.Clear();
            OpResponseData.OperationCode = operationRequest.OperationCode;
            if (!isExist)
            {
                Utility.Debug.LogInfo("==========\n  before add UUID ：" +userJson +"\n"+ userObj.UUID + "\n================");

                //添加输入的用户和密码进数据库
                userObj =  NHibernateQuerier.Insert(userObj);
                Utility.Debug.LogInfo("==========\n after add UUID ：" + userJson + "\n" + userObj.UUID+"\n================");
                NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", userObj.UUID);
                bool userRoleExist = NHibernateQuerier.Verify<UserRole>(nHCriteriaUUID);
                if (!userRoleExist)
                {
                    var userRole = new UserRole() { UUID = userObj.UUID };
                    NHibernateQuerier.Insert(userRole);
                }
            OpResponseData.ReturnCode = (short)ReturnCode.Success;//返回成功
                GameManager.ReferencePoolManager.Despawns(nHCriteriaUUID);
            }
            else//否者这个用户被注册了
            {
                OpResponseData.ReturnCode = (short)ReturnCode.Fail;//返回失败
            }
            // 把上面的结果给客户端
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAccount);
            return OpResponseData;
        }
    }
}
