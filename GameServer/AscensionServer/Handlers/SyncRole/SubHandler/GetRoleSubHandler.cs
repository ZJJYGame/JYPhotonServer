using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
namespace AscensionServer
{
    public class GetRoleSubHandler : SyncRoleSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string account = Utility.ToObject<User>(Convert.ToString (Utility.GetValue(dict, (byte)ObjectParameterCode.User))).Account;
            NHCriteria nHCriteriaAccount = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", account);
            string _uuid = Singleton<NHManager>.Instance.CriteriaGet<User>(nHCriteriaAccount).UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", _uuid);
            UserRole userRole = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
            Utility.Assert.NotNull(userRole.RoleIDArray,()=> 
            {
                var userRoleObj = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
                string roleIDListJson = userRoleObj.RoleIDArray;
                List<string> roleIDlist;
                List<Role> roleObjList = new List<Role>();
                List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
                Utility.Assert.NotNull(roleIDListJson, () =>
                {
                    roleIDlist = new List<string>();
                    roleIDlist = Utility.ToObject<List<string>>(roleIDListJson);
                    for (int i = 0; i < roleIDlist.Count; i++)
                    {
                        NHCriteria tmpCriteria = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", int.Parse(roleIDlist[i]));
                        Role tmpRole = Singleton<NHManager>.Instance.CriteriaGet<Role>(tmpCriteria);
                        roleObjList.Add(tmpRole);
                        nHCriteriaList.Add(tmpCriteria);
                    }
                });
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RoleList, Utility.ToJson(roleObjList));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaList);
            },
                ()=>SetResponseData(()=> {
                SubDict.Add((byte)ParameterCode.RoleList, Utility.ToJson(new List<string>()));
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Empty;
            }));
            // 把上面的结果给客户端
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID, nHCriteriaAccount);

        }
    }
}
