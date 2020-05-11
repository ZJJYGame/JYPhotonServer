/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 选择角色
*/
using AscensionProtocol;
using AscensionServer.Model;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace AscensionServer.Handler
{
    class SelectRoleHandler :BaseHandler
    {
        public SelectRoleHandler()
        {
            opCode = AscensionProtocol.OperationCode.SelectRole;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string username = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;

             string _uuid = Singleton<UserManager>.Instance.GetUUid(username);
             AscensionServer.log.Info(">>>>>>>>>>>>>" + Singleton<UserRoleManager>.Instance.GetArray(_uuid));

            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
            UserRole userRole=Singleton<NHManager>.Instance.CriteriaGet<UserRole>( "UUID", _uuid);
         


            if (string.IsNullOrEmpty(Singleton<UserRoleManager>.Instance.GetArray(_uuid)))
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                //data.Add((byte)ParameterCode.UserCode.RoleList, Utility.Serialize(new List<string>()));
                data.Add((byte)ParameterCode.UserCode.RoleList, Utility.ToJson(new List<string>()));
                responser.Parameters = data;
            }
            else
            {
                string[] str = Singleton<UserRoleManager>.Instance.GetArray(_uuid).Split(new char[] { ',' });
                List<string> strList = new List<string>(str);
                List<string> rList = new List<string>();
                foreach (var pName in strList)
                {
                    rList.Add(Singleton<RoleManager>.Instance.GetRoleId(Convert.ToInt32(pName)));
                }
                //string rolelistString = Utility.Serialize(rList);
                string rolelistString = Utility.ToJson(rList);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, rolelistString);
                responser.Parameters = data;
            }
          

            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);
        }
    }
}
