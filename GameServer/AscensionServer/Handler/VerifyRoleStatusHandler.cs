using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;


namespace AscensionServer.Handler
{
    class VerifyRoleStatusHandler : BaseHandler
    {
        public VerifyRoleStatusHandler()
        {
            opCode = AscensionProtocol.OperationCode.VerifyRole;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string rolestatusJson = Utility.GetValue(operationRequest.Parameters,(byte)ObjectParameterCode.Role)as string;
            AscensionServer.log.Info(">>>>>>>>>>>>传输过来更新的战斗数据:"+ rolestatusJson +"<<<<<<<<<<<");
            var rolestatusObj = Utility.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleStatus>(nHCriteriaRoleStatue);
            OperationResponse operationResponse = new OperationResponse(operationRequest.OperationCode);
            if (exist)
            {
                //Singleton<NHManager>.Instance.Update(rolestatusObj);
               Singleton<NHManager>.Instance.Update(Utility.ToObject<RoleStatus>(rolestatusJson));
                String VerifyRoleStatus = Utility.ToJson(rolestatusJson);
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ObjectParameterCode.RoleStatus, VerifyRoleStatus);
                operationResponse.Parameters = data;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
                
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(operationResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleStatue);
        }
    }



}
