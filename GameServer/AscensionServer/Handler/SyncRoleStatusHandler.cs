/*
*Author : Don
*Since 	:2020-04-18
*Description  : 请求角色数值处理者，返回角色的status
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
    public class SyncRoleStatusHandler : HandlerBase
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleStatus;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer.log.Info("------------------------------------" + "roleJson  : " + roleJson + "---------------------------------------");
            OpResponse.OperationCode = operationRequest.OperationCode;
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            ResponseData.Clear();
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleId);
            if (exist)
            {
                RoleStatus roleStatus = Singleton<NHManager>.Instance.CriteriaGet<RoleStatus>(nHCriteriaRoleId);
                AscensionServer.log.Info("------------------------------------" + "RoleStatus  : " + roleStatus + "---------------------------------------");
                string roleStatusJson = Utility.ToJson(roleStatus);
                ResponseData.Add((byte)ObjectParameterCode.RoleStatus, roleStatusJson);
                OpResponse.Parameters = ResponseData;
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleId);
        }
    }
}
