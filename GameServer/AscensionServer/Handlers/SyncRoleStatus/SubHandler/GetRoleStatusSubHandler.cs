using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class GetRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            
            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            if (RedisHelper.Hash.HashExistAsync("Role", roleObj.RoleID.ToString()).Result&& RedisHelper.Hash.HashExistAsync("RoleStatus", roleObj.RoleID.ToString()).Result)
            {
              var roleStatus=  RedisHelper.Hash.HashGetAsync<RoleStatus>("RoleStatus", roleObj.RoleID.ToString());
            }
            else
            {
                #region MySql
                NHCriteria nHCriteriaRoleId = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
                bool exist = NHibernateQuerier.Verify<Role>(nHCriteriaRoleId);
                if (exist)
                {
                    Utility.Debug.LogInfo("------------------------------------" + "获取人物数据  : " + roleJson + "---------------------------------------");
                    //AscensionServer.Instance.Online(peer, roleObj);
                    RoleStatus roleStatus = NHibernateQuerier.CriteriaSelect<RoleStatus>(nHCriteriaRoleId);
                    Utility.Debug.LogInfo("------------------------------------GetRoleStatusSubHandler\n" + "RoleStatus  : " + roleStatus + "\nGetRoleStatusSubHandler---------------------------------------");
                    string roleStatusJson = Utility.Json.ToJson(roleStatus);
                    RoleRing roleRing = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriaRoleId);
                    SetResponseParamters(() => { subResponseParameters.Add((byte)ParameterCode.RoleStatus, roleStatusJson); subResponseParameters.Add((byte)ParameterCode.Inventory, Utility.Json.ToObject<Dictionary<int, int>>(roleRing.RingIdArray)); });
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                    GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleId);
                }
                else
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                #endregion
            }
            return operationResponse;
        }
    }
}
