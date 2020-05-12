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
            opCode = AscensionProtocol.OperationCode.SyncRoleStatus;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role)as string;
            AscensionServer.log.Info("------------------------------------" + "roleJson  : " + roleJson + "---------------------------------------");
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            var roleObj = Utility.ToObject<Role>(roleJson);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(new NHCriteria() { PropertyName = "RoleId", Value = roleObj.RoleId });
            if (exist)
            {
                RoleStatus roleStatus = Singleton<NHManager>.Instance.CriteriaGet<RoleStatus>(new NHCriteria() { PropertyName = "RoleId", Value =  roleObj.RoleId });
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
        }
    }
}
