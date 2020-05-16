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
            opCode = AscensionProtocol.OperationCode.VerifyRoleStatus;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string rolestatusJson = Utility.GetValue(operationRequest.Parameters,(byte)ObjectParameterCode.Role)as string;
            AscensionServer.log.Info(">>>>>>>>>>>>传输过来更新的战斗数据:"+ rolestatusJson +"<<<<<<<<<<<");
            OperationResponse operationResponse = new OperationResponse(operationRequest.OperationCode);
            var rolestatusObj = Utility.ToObject<RoleStatus>(rolestatusJson);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleStatus>(new NHCriteria() { PropertyName = "RoleId", Value = rolestatusObj.RoleId });
            if (exist)
            {
                Singleton<NHManager>.Instance.Update<RoleStatus>(rolestatusObj);
                Dictionary<byte, object> data = new Dictionary<byte, object>();

                data.Add((byte)ObjectParameterCode.RoleStatus, rolestatusJson);
                operationResponse.Parameters = data;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(operationResponse, sendParameters);
        }
    }



}
