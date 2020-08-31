using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
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
            ResetResponseData(operationRequest);
            string roleAssetsJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAssets));
            AscensionServer._Log.Info(">>>>>>>>>>>>>更新财产相关信息：" + roleAssetsJson + ">>>>>>>>>>>>>>>>>>>>>>");
            var roleAssetsObj = Utility.Json.ToObject<RoleAssets>(roleAssetsJson);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleAssetsObj.RoleID);
            bool roleExist = ConcurrentSingleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleAssets>(nHCriteriaRoleID);

            if (roleExist && roleAssetsExist)
            {
                var assetsServer = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);

                if (roleAssetsObj.XianYu != 0 && assetsServer.XianYu >= 0)
                {
                    assetsServer.XianYu += roleAssetsObj.XianYu;
                    if ((assetsServer.XianYu + roleAssetsObj.XianYu < 0))
                        assetsServer.XianYu = 0;
                }
                if (roleAssetsObj.SpiritStonesLow != 0 && assetsServer.SpiritStonesLow >= 0)
                {
                    assetsServer.SpiritStonesLow += roleAssetsObj.SpiritStonesLow;
                    if ((assetsServer.SpiritStonesLow + roleAssetsObj.SpiritStonesLow < 0))
                        assetsServer.SpiritStonesLow = 0;
                }

                ConcurrentSingleton<NHManager>.Instance.Update<RoleAssets>(new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesHigh = assetsServer.SpiritStonesHigh, SpiritStonesLow = assetsServer.SpiritStonesLow, SpiritStonesMedium = assetsServer.SpiritStonesMedium, XianYu = assetsServer.XianYu });
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);
        }
    }
}
