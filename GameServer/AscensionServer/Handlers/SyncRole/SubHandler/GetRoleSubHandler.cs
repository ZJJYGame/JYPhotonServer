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

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string account = Utility.Json.ToObject<User>(Convert.ToString (Utility.GetValue(dict, (byte)ParameterCode.User))).Account;
            NHCriteria nHCriteriaAccount = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("Account", account);
            string _uuid = NHibernateQuerier.CriteriaSelect<User>(nHCriteriaAccount).UUID;
            NHCriteria nHCriteriaUUID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("UUID", _uuid);
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
                        NHCriteria tmpCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", int.Parse(roleIDlist[i]));
                        Role tmpRole = NHibernateQuerier.CriteriaSelect<Role>(tmpCriteria);
                        roleObjList.Add(tmpRole);
                        nHCriteriaList.Add(tmpCriteria);
                    }
                }
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.RoleSet, Utility.Json.ToJson(roleObjList));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaList);
            }
            else
            {
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.RoleSet, Utility.Json.ToJson(new List<string>()));
                    operationResponse.ReturnCode = (byte)ReturnCode.Empty;
                });
            }
            // 把上面的结果给客户端
            CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaUUID, nHCriteriaAccount);
            return operationResponse;
        }
    }
}


