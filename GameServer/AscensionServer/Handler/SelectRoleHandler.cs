/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 选择角色
*/
using AscensionProtocol;
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

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            string rolename = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;
         
            string _uuid = Singleton<UserManager>.Instance.GetUUid(rolename);
          
            AscensionServer.log.Info(">>>>>>>>>>>>>" +  Singleton<UserRoleManager>.Instance.GetArray(_uuid));
            
            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
             
            if (string.IsNullOrEmpty(Singleton<UserRoleManager>.Instance.GetArray(_uuid)))
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, "用户名下还没有角色");
                responser.Parameters = data;
                return;
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
                string rolelistString = Utility.Serialize(rList);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, rolelistString);
                responser.Parameters = data;
            }
            #region 选择角色，给客户端发数据


            ////如果没有查询到 需要玩家注册新角色
            //if (rolelist.Count != 0)
            //{
            //   // 给客户端响应
            //    foreach (var p in rolelist)
            //    {
            //        rList.Add(p.RoleName);s
            //        AscensionServer.log.Info(p.RoleName);
            //    }
            //    string rolelistString = Utility.Serialize(rList);
            //    Dictionary<byte, object> data = new Dictionary<byte, object>();
            //    data.Add((byte)ParameterCode.UserCode.RoleList, rolelistString);
            //    responser.Parameters = data;
            //}
            //else
            //{
            //    Dictionary<byte, object> data = new Dictionary<byte, object>();
            //    data.Add((byte)ParameterCode.UserCode.RoleList, "用户名下还没有角色");
            //    responser.Parameters = data;
            //}
            #endregion

            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);
        }
    }
}
