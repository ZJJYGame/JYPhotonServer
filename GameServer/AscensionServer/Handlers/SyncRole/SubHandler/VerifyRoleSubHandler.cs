using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
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

            //废弃代码
            /*
            string rolestatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>VerifyRoleStatusHandler\n传输过来更新的战斗数据:" + rolestatusJson + "VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var rolestatusObj = Utility.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            Owner. ResponseData.Clear();
            bool exist = Singleton<NHManager>.Instance.Verify<RoleStatus>(nHCriteriaRoleStatue);
           Owner. OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
                Singleton<NHManager>.Instance.Update(rolestatusObj);
               Owner. OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
               Owner. OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner. OpResponse, sendParameters);*/
        }
    }
}
