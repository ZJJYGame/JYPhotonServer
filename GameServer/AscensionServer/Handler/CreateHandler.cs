/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 创建角色处理者
*/
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Photon.SocketServer;
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
            string rolename = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Rolename) as string;
            string linggenlist = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.LingGen.LingGenList) as string;
            string account = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Username) as string;


            //整个传输，测试
            string roleJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role) as string;

            Role roleTmp = Utility.ToObject<Role>(roleJson);
            var verifyRole = Singleton<RoleManager>.Instance.GetByRolename(roleTmp.RoleName);
            if (verifyRole != null)
                AscensionServer.log.Info("--------------------------------VerifyRole not null");

            Role role = Singleton<RoleManager>.Instance.GetByRolename(rolename);//根据username查询数据
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            string str_uuid = peer.UUID;
            string strArray = Singleton<UserRoleManager>.Instance.GetArray(str_uuid);

            string roleStatusJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.RoleStatus) as string;
            //TODO 精简createHandler代码
            string str = "";
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                //添加输入的用户进数据库
                role = new Role() { RoleName = rolename, RoleRoot = linggenlist, RoleLevel = 0, RoleExp =0 };
                var rolestatus = Utility.ToObject<Model.RoleStatus>(roleStatusJson);
                string roleId = Singleton<RoleManager>.Instance.AddRole(role).RoleId.ToString();
                if (!string.IsNullOrEmpty(strArray))
                {
                    //roleId = Singleton<RoleManager>.Instance.AddRole(role).RoleId.ToString();
                    str = strArray + "," +roleId;
                }
                else
                {
                    //str = strArray+ Singleton<RoleManager>.Instance.AddRole(role).RoleId.ToString();
                    str = strArray + roleId;
                }
                rolestatus.RoleId = int.Parse(roleId);
                Singleton<NHManager>.Instance.Add(rolestatus);
                Singleton<UserRoleManager>.Instance.UpdateStr(new UserRole() { Role_Id_Array = str, UUID = str_uuid });
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
