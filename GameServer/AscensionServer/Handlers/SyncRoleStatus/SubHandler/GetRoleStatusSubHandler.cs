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
            var dict = ParseSubDict(operationRequest);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info("------------------------------------" + "roleJson  : " + roleJson + "---------------------------------------");
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleId);
            if (exist)
            {
                RoleStatus roleStatus = Singleton<NHManager>.Instance.CriteriaGet<RoleStatus>(nHCriteriaRoleId);
                AscensionServer._Log.Info("------------------------------------GetRoleStatusSubHandler\n" + "RoleStatus  : " + roleStatus + "\nGetRoleStatusSubHandler---------------------------------------");
                string roleStatusJson = Utility.ToJson(roleStatus);
                SetResponseData(() => {SubDict.Add((byte)ObjectParameterCode.RoleStatus, roleStatusJson);});
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleId);
        }
    }
}
