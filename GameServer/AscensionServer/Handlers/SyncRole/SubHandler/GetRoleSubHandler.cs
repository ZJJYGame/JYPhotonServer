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
    public class GetRoleSubHandler : SyncRoleSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string account = Utility.Json.ToObject<User>(Convert.ToString (Utility.GetValue(dict, (byte)ParameterCode.User))).Account;
            NHCriteria nHCriteriaAccount = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", account);
            string _uuid = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaAccount).UUID;
            NHCriteria nHCriteriaUUID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", _uuid);
            UserRole userRole = NHibernateQuerier.CriteriaSelect<UserRole>(nHCriteriaUUID);
            if (!string.IsNullOrEmpty(userRole.RoleIDArray))
            {
                var userRoleObj = NHibernateQuerier.CriteriaSelect<UserRole>(nHCriteriaUUID);
                string roleIDListJson = userRoleObj.RoleIDArray;
                List<string> roleIDlist;
                List<Role> roleObjList = new List<Role>();
                List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(roleIDListJson))
                {
                    roleIDlist = new List<string>();
                    roleIDlist = Utility.Json.ToObject<List<string>>(roleIDListJson);
                    for (int i = 0; i < roleIDlist.Count; i++)
                    {
                        NHCriteria tmpCriteria = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", int.Parse(roleIDlist[i]));
                        Role tmpRole = NHibernateQuerier.CriteriaSelect<Role>(tmpCriteria);
                        roleObjList.Add(tmpRole);
                        nHCriteriaList.Add(tmpCriteria);
                    }
                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RoleSet, Utility.Json.ToJson(roleObjList));
                    Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Success;
                });
                GameManager.ReferencePoolManager.Despawns(nHCriteriaList);
            }
            else
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RoleSet, Utility.Json.ToJson(new List<string>()));
                    Owner.OpResponseData.ReturnCode = (byte)ReturnCode.Empty;
                });
            }
            // 把上面的结果给客户端
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaUUID, nHCriteriaAccount);

        }
    }
}
