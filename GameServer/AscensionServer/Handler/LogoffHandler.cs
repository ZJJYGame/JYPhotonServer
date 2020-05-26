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
    public class LogoffHandler : BaseHandler
    {
        public LogoffHandler()
        {
            opCode = OperationCode.Logoff;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
         
            string userJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.User) );
            var userObj = Utility.ToObject<User>(userJson);
            AscensionServer.log.Info("登出的账号"+userJson+">>>>>>>>>>>>>>>>>>");
            //NHCriteria nHCriteriaAccount = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", userObj.Account);
            //NHCriteria nHCriteriaPassword = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Password", userObj.Password);
            //bool verified = Singleton<NHManager>.Instance.Verify<User>(nHCriteriaAccount, nHCriteriaPassword);
            bool verified= peer.User.Equals(userObj);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (verified)
            {
                response.ReturnCode =(short) ReturnCode.Success;
            }
            else
            {
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(response, sendParameters);
            //Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaAccount, nHCriteriaPassword);
        }
    }
}
