using AscensionProtocol;
using AscensionServer.Model;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // RoleManager roleManager = new RoleManager();
            //Role role = roleManager.GetByRolename(rolename);//根据username查询数据
            //ICollection<Role> rolelist = roleManager.GetAllRoles();
            ////AscensionServer.log.Info(rolelist.Count);
            ///
            UserManager manager = new UserManager();
            string _uuid = manager.GetUUid(rolename);
            User_Role user_Role = new User_Role();
            RoleManager roleManager = new RoleManager();
            //user_Role.GetArray(_uuid);
            
            AscensionServer.log.Info(">>>>>>>>>>>>>" +  user_Role.GetArray(_uuid));
            
            OperationResponse responser = new OperationResponse(operationRequest.OperationCode);
             
            if (string.IsNullOrEmpty(user_Role.GetArray(_uuid)))
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, "用户名下还没有角色");
                responser.Parameters = data;
                return;
            }
            else
            {
                string[] str = user_Role.GetArray(_uuid).Split(new char[] { ',' });
                List<string> strList = new List<string>(str);
                List<string> rList = new List<string>();
                foreach (var pName in strList)
                {
                    rList.Add(roleManager.GetRoleId(Convert.ToInt32(pName)));
                }
                string rolelistString = Utility.Serialize(rList);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.UserCode.RoleList, rolelistString);
                responser.Parameters = data;
            }
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


            // 把上面的结果给客户端
            peer.SendOperationResponse(responser, sendParameters);
        }
    }
}
