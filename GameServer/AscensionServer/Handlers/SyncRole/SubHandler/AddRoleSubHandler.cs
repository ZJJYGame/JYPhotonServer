using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 添加新角色子操作码
    /// </summary>
    public class AddRoleSubHandler : SyncRoleSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleJsonTmp = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Role));
            Role roleTmp = Utility.Json.ToObject<Role>(roleJsonTmp);
            NHCriteria nHCriteriaRoleName = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                AscensionServer._Log.Info("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = Singleton<NHManager>.Instance.CriteriaSelect<Role>(nHCriteriaRoleName);//根据username查询数据
            string str_uuid = peer.PeerCache.UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = Singleton<NHManager>.Instance.CriteriaSelect<UserRole>(nHCriteriaUUID);
            string roleJson = userRole.RoleIDArray;
            string roleStatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RoleStatus));
            //如果没有查询到代表角色没被注册过可用

            if (role == null)
            {
                List<string> roleList = new List<string>();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList = Utility.Json.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.Json.ToObject<RoleStatus>(roleStatusJson);
                role = Singleton<NHManager>.Instance.Insert<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                Singleton<NHManager>.Instance.Insert(rolestatus);
                Singleton<NHManager>.Instance.Insert(new RoleAssets() { RoleID = rolestatus.RoleID });
                Singleton<NHManager>.Instance.Insert(new OnOffLine() { RoleID = rolestatus.RoleID });
                var userRoleJson = Utility.Json.ToJson(roleList);
                Singleton<NHManager>.Instance.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                Owner.ResponseData.Add((byte)ObjectParameterCode.Role, Utility.Json.ToJson(role));
                Owner.OpResponse.Parameters = Owner.ResponseData;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID, nHCriteriaRoleName);
        }


    }
}
