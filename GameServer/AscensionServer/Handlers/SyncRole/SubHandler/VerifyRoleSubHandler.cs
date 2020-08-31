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
    public class VerifyRoleSubHandler : SyncRoleSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rolestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleStatus));
            Utility.Debug.LogInfo(">>>>>>>>>>>>VerifyRoleStatusHandler\n传输过来更新的战斗数据:" + rolestatusJson + "VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var rolestatusObj = Utility.Json.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            var result = ConcurrentSingleton<NHManager>.Instance.Verify<RoleStatus>(nHCriteriaRoleStatue);
            if (result)
            {
                ConcurrentSingleton<NHManager>.Instance.Update(rolestatusObj);
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Success);
            }
            else
            {
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}
