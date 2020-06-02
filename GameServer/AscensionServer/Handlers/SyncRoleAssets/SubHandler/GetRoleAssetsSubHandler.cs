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
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = InitSubOpDict(operationRequest);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>SyncRoleAssetsHandler\n" + roleJson + "\n SyncRoleAssetsHandler >>>>>>>>>>>>>>>>>>>>>>");
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);

            if (exist)
            {
                var result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);
                if (result == null)
                {
                    Singleton<NHManager>.Instance.Add(new RoleAssets()
                    {
                        RoleID = roleObj.RoleID,
                        SpiritStonesHigh = 0,
                        SpiritStonesLow = 0,
                        SpiritStonesMedium = 0,
                        XianYu = 0
                    });
                    result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);
                }
                string roleAssetsJson = Utility.ToJson(result);
                AscensionServer._Log.Info(">>>>>>>>>>>>>>roleAssetsJson \n" + roleAssetsJson + "\n roleAssetsJson<<<<<<<<<<<<<<");

                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                Owner.ResponseData.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
                Owner.OpResponse.Parameters = Owner.ResponseData;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
