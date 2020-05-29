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
    class VerifyRoleStatusHandler : HandlerBase
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.VerifyRole;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string rolestatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer.log.Info(">>>>>>>>>>>>VerifyRoleStatusHandler\n传输过来更新的战斗数据:" + rolestatusJson + "VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var rolestatusObj = Utility.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            ResponseData.Clear();
            bool exist = Singleton<NHManager>.Instance.Verify<RoleStatus>(nHCriteriaRoleStatue);
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
                Singleton<NHManager>.Instance.Update(rolestatusObj);
                OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(OpResponse, sendParameters);
        }
    }
}
