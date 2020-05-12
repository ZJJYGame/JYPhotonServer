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
            string account= Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Account) as string;
             string _uuid = Singleton<NHManager>.Instance.CriteriaGet<User>(new NHCriteria() { PropertyName="Account",Value= account }).UUID;
            UserRole userRole = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(new NHCriteria() { PropertyName = "UUID", Value = _uuid });
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (string.IsNullOrEmpty(userRole.Role_Id_Array))
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, Utility.ToJson(new List<string>()));
                response.Parameters = data;
            }
            else
            {
                var userRoleObj= Singleton<NHManager>.Instance.CriteriaGet<UserRole>(new NHCriteria() { PropertyName = "UUID", Value = _uuid });
                string roleListJson = userRoleObj.Role_Id_Array;
                List<string> rolelist;
                List<Role> roleObjList=new List<Role>();
                if (!string.IsNullOrEmpty(roleListJson))
                {
                    rolelist = new List<string>();
                    rolelist = Utility.ToObject<List<string>>(roleListJson);
                    for (int i = 0; i < rolelist.Count; i++)
                    {
                        Role tmpRole = Singleton<NHManager>.Instance.CriteriaGet<Role>(new NHCriteria() { PropertyName = "RoleId", Value =int.Parse( rolelist[i]) });
                        roleObjList.Add(tmpRole);
                    }
                }
                Dictionary<byte, object> roleData = new Dictionary<byte, object>();
                roleData.Add((byte)ObjectParameterCode.RoleData, Utility.ToJson(roleObjList));
                response.Parameters = roleData;
            }
            // 把上面的结果给客户端
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
