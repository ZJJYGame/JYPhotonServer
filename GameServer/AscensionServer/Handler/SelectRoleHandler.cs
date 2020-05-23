/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 选择角色
*/
using AscensionProtocol;
using AscensionServer.Model;
using Photon.SocketServer;
using System;
using System.Collections.Generic;

namespace AscensionServer.Handler
{
    class SelectRoleHandler :BaseHandler
    {
        public SelectRoleHandler()
        {
            opCode = AscensionProtocol.OperationCode.SelectRole;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string account = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Account) );
            NHCriteria nHCriteriaAccount =Singleton< ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("Account", account);
            string _uuid = Singleton<NHManager>.Instance.CriteriaGet<User>(nHCriteriaAccount).UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", _uuid);
            UserRole userRole = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (string.IsNullOrEmpty(userRole.RoleIDArray))
            {
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.RoleList, Utility.ToJson(new List<string>()));
                response.Parameters = data;
                response.ReturnCode = (byte)ReturnCode.Empty;
            }
            else
            {
                var userRoleObj= Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
                string roleIDListJson = userRoleObj.RoleIDArray;
                List<string> roleIDlist;
                List<Role> roleObjList=new List<Role>();
                List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(roleIDListJson))
                {
                    roleIDlist = new List<string>();
                    roleIDlist = Utility.ToObject<List<string>>(roleIDListJson);
                    for (int i = 0; i < roleIDlist.Count; i++)
                    {
                        NHCriteria tmpCriteria = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", int.Parse(roleIDlist[i]));
                        AscensionServer.log.Info("````````````````````````````tmpCriteria" + tmpCriteria.ToString() + "``````````````````````````````````");
                        Role tmpRole = Singleton<NHManager>.Instance.CriteriaGet<Role>(tmpCriteria);
                        roleObjList.Add(tmpRole);
                        nHCriteriaList.Add(tmpCriteria);
                    }
                }
                Dictionary<byte, object> roleData = new Dictionary<byte, object>();
                roleData.Add((byte)ParameterCode.RoleList, Utility.ToJson(roleObjList));
                response.Parameters = roleData;
                response.ReturnCode = (byte)ReturnCode.Success;
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaList);
            }
            // 把上面的结果给客户端
            peer.SendOperationResponse(response, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID, nHCriteriaAccount);
        }
    }
}
