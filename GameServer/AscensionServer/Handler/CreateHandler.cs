/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 创建角色处理者
*/
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Photon.SocketServer;
using System.Collections.Generic;
namespace AscensionServer.Handler
{
    class CreateHandler:BaseHandler
    {
        public CreateHandler()
        {
            opCode = AscensionProtocol.OperationCode.CreateRole;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJsonTmp = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role) as string;
            Role roleTmp = Utility.ToObject<Role>(roleJsonTmp);
            var isExisted = Singleton<NHManager>.Instance.Verify<Role>(new NHCriteria() { PropertyName="RoleName",Value=roleTmp.RoleName});
            if (isExisted)
                AscensionServer.log.Info("----------------------------  Role >>Role name:+"+roleTmp.RoleName+" already exist !!!  ---------------------------------");
            Role role = Singleton<NHManager>.Instance.CriteriaGet<Role>(new NHCriteria() { PropertyName = "RoleName", Value = roleTmp.RoleName });//根据username查询数据
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            string str_uuid = peer.UUID;
            var userRole= Singleton<NHManager>.Instance.CriteriaGet<UserRole>(new NHCriteria() { PropertyName = "UUID", Value = str_uuid });
            string roleJson = userRole.Role_Id_Array;

            string roleStatusJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.RoleStatus) as string;
            //TODO 精简createHandler代码
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                List<string> roleList = new List<string>();
                if(!string.IsNullOrEmpty(roleJson))
                roleList = Utility.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.ToObject<RoleStatus>(roleStatusJson);
                string roleId = Singleton<NHManager>.Instance.Add<Role>(role).RoleId.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                {
                    roleList.Add(roleId);
                }
                else
                {
                    roleList.Add(roleId);
                }
                rolestatus.RoleId = int.Parse(roleId);
                Singleton<NHManager>.Instance.Add(rolestatus);
                var userRoleJson = Utility.ToJson(roleList);
                Singleton<NHManager>.Instance.Update(new UserRole() { Role_Id_Array = userRoleJson, UUID = str_uuid });
                response.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(response, sendParameters);
        }
    }
}
