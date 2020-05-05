using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionServer.Model;
using Photon.SocketServer;

namespace AscensionServer.Handler
{
    class CreateHandle:BaseHandler
    {
        public CreateHandle()
        {
            opCode = AscensionProtocol.OperationCode.CreateRole;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, MyClientPeer peer)
        {
            string rolename = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Rolename) as string;
            string linggenlist = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.LingGen.LingGenList) as string;
            string account = Utility.GetValue<byte, object>(operationRequest.Parameters, (byte)ParameterCode.UserCode.Username) as string;
            Role role = Singleton<RoleManager>.Instance.GetByRolename(rolename);//根据username查询数据
            OperationResponse response = new Photon.SocketServer.OperationResponse(operationRequest.OperationCode);
            string str_uuid = peer.uuid;
            string strArray = Singleton<User_RoleManager>.Instance.GetArray(str_uuid);
            string str = "";
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                //添加输入的用户进数据库
                role = new Role() { RoleName = rolename, RoleRoot = linggenlist, RoleLevel = 122, RoleExp = 122 };
                if (!string.IsNullOrEmpty(strArray))
                {
                     str = strArray + "," + Singleton<RoleManager>.Instance.AddRole(role).RoleId.ToString();
                    Singleton<User_RoleManager>.Instance.UpdateStr(new User_Role() { Role_Id_Array = str ,UUID = str_uuid});
                }
                else
                {
                    Singleton<User_RoleManager>.Instance.AddStr(new User_Role() { Role_Id_Array = str });
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
