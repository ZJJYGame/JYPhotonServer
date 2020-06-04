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
    public class GetRoleAssetsSubHandler : SyncRoleAssetsSubHandler
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
            AscensionServer._Log.Info(">>>>>>>>>>>>>GetRoleAssetsSubHandler\n" + roleJson + "\n GetRoleAssetsSubHandler >>>>>>>>>>>>>>>>>>>>>>");
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var obj =Singleton<NHManager>.Instance.CriteriaGet<Role>(nHCriteriaRoleID);
            Utility.Assert.NotNull(obj,() =>
                {
                    var result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);
                    Utility.Assert.IsNull(result, () =>
                    {
                        AscensionServer._Log.Info(">>>>>>>>>>>>>\n GetRoleAssetsSubHandler  " + roleObj.RoleID+ "  GetRoleAssetsSubHandler  \n >>>>>>>>>>>>>>>>>>>>>>");
                       result= Singleton<NHManager>.Instance.Add(new RoleAssets() { RoleID = roleObj.RoleID});
                    });
                    string roleAssetsJson = Utility.ToJson(result);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }, () => Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail);
          
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
