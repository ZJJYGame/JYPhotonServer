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
    public class UpdateRoleAssetsSubHandler : SyncRoleAssetsSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleAssetsJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RoleAssets));
            var roleAssetsObj = Utility.ToObject<RoleAssets>(roleAssetsJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleAssetsObj.RoleID);
            bool roleExist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = Singleton<NHManager>.Instance.Verify<RoleAssets>(nHCriteriaRoleID);
            if (roleExist && roleAssetsExist)
            {
                try
                {
                    Singleton<NHManager>.Instance.Update<RoleAssets>(roleAssetsObj);
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                }
                catch 
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                }
                peer.SendOperationResponse(Owner.OpResponse, sendParameters);
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
            }
        }
    }
}
