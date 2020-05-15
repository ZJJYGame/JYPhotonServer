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
    class CreateHandler : BaseHandler
    {
        public CreateHandler()
        {
            opCode = AscensionProtocol.OperationCode.CreateRole;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            NHCriteria[] nHCriterias = new NHCriteria[2];
            string roleJsonTmp = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role) as string;
            Role roleTmp = Utility.ToObject<Role>(roleJsonTmp);
            NHCriteria nHCriteriaRoleName = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                AscensionServer.log.Info("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = Singleton<NHManager>.Instance.CriteriaGet<Role>(nHCriteriaRoleName);//根据username查询数据
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            string str_uuid = peer.UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
            string roleJson = userRole.RoleIDArray;
            string roleStatusJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.RoleStatus) as string;
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                List<string> roleList = new List<string>();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList = Utility.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.ToObject<RoleStatus>(roleStatusJson);
                role = Singleton<NHManager>.Instance.Add<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                Singleton<NHManager>.Instance.Add(rolestatus);
                var userRoleJson = Utility.ToJson(roleList);
                Singleton<NHManager>.Instance.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                response.ReturnCode = (short)ReturnCode.Success;
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ObjectParameterCode.Role, Utility.ToJson(role));
                response.Parameters = data;
            }
            else
            {
                response.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(response, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID,nHCriteriaRoleName);
        }
    }
}
