using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    public class GetRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info("------------------------------------" + "roleJson  : " + roleJson + "---------------------------------------");
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            Owner.ResponseData.Clear();
            Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleId);
            if (exist)
            {
                RoleStatus roleStatus = Singleton<NHManager>.Instance.CriteriaGet<RoleStatus>(nHCriteriaRoleId);
                AscensionServer._Log.Info("------------------------------------" + "RoleStatus  : " + roleStatus + "---------------------------------------");
                string roleStatusJson = Utility.ToJson(roleStatus);
                Owner.ResponseData.Add((byte)ObjectParameterCode.RoleStatus, roleStatusJson);
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleId);
        }
    }
}
