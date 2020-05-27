/*
*Author : Don
*Since 	:2020-04-18
*Description  : 请求角色数值处理者
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class SyncRoleStatusHandler : BaseHandler
    {
        public SyncRoleStatusHandler()
        {
            OpCode = AscensionProtocol.OperationCode.SyncRoleStatus;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer.log.Info("------------------------------------" + "roleJson  : " + roleJson + "---------------------------------------");
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleId= Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleId);
            if (exist)
            {
                RoleStatus roleStatus = Singleton<NHManager>.Instance.CriteriaGet<RoleStatus>(nHCriteriaRoleId);
                AscensionServer.log.Info("------------------------------------" + "RoleStatus  : " + roleStatus + "---------------------------------------");
                string roleStatusJson = Utility.ToJson(roleStatus);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ObjectParameterCode.RoleStatus, roleStatusJson);
                response.Parameters = data;
                response.ReturnCode = (short)ReturnCode.Success;
            }
            else
                response.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(response, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleId);
        }
    }
}
