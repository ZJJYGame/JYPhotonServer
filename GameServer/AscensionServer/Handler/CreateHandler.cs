/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 创建角色处理者
*/
using AscensionProtocol;
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
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            string rolename = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Rolename) as string;
            string linggenlist = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.LingGen.LingGenList) as string;
            string account = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.UserCode.Username) as string;
            Role role = Singleton<RoleManager>.Instance.GetByRolename(rolename);//根据username查询数据
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            string str_uuid = peer.uuid;
            //string strArray = Singleton<UserRoleManager>.Instance.GetArray(str_uuid);
            UserRole userRole= Singleton< UserRoleManager>.Instance.CriteriaGet<UserRole>("UUID",str_uuid);
            string strArray = userRole==null?   "": userRole.Role_Id_Array;
            string str = "";
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                //添加输入的用户进数据库
                role = new Role() { RoleName = rolename, RoleRoot = linggenlist, RoleLevel = 122, RoleExp = 122 };
                if (!string.IsNullOrEmpty(strArray))
                {
                    //str = strArray + "," + Singleton<RoleManager>.Instance.AddRole(role).RoleId.ToString();
                    Singleton<NHManager>.Instance.Add(role);
                    str = Utility.Text.Format(strArray, ",", Singleton<RoleManager>.Instance.Get<Role>(role.RoleId).RoleId);
                    //Singleton<UserRoleManager>.Instance.UpdateStr(new UserRole() { Role_Id_Array = str, UUID = str_uuid });
                    Singleton<NHManager>.Instance.Update(new UserRole() { Role_Id_Array = str, UUID = str_uuid });
                }
                else
                {
                    //TODO 创建角色，用户角色表存储数据
                    Singleton<NHManager>.Instance.Add(role);
                    str = Utility.Text.Format(strArray, ",", Singleton<RoleManager>.Instance.Get<Role>(role.RoleId).RoleId);
                    //Singleton<UserRoleManager>.Instance.Add(new UserRole() { Role_Id_Array = str });

                    Singleton<UserRoleManager>.Instance.Add(new UserRole() {UUID="", Role_Id_Array = str });
                }
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
