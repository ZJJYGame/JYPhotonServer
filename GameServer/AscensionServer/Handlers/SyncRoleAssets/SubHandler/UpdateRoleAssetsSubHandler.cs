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
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleAssetsObj.RoleID);
            bool roleExist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = Singleton<NHManager>.Instance.Verify<RoleAssets>(nHCriteriaRoleID);
            long SpiritStonesLow = 0;
            long SpiritStonesHigh = 0;
            long SpiritStonesMedium = 0;
            long XianYu = 0;
            if (roleExist && roleAssetsExist)
            {
                var assetsServer = Singleton<NHManager>.Instance.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);
                SpiritStonesLow = assetsServer.SpiritStonesLow;
                if (roleAssetsObj.SpiritStonesLow > 0)
                    SpiritStonesLow = roleAssetsObj.SpiritStonesLow + assetsServer.SpiritStonesLow;
                SpiritStonesHigh = assetsServer.SpiritStonesHigh;
                if (roleAssetsObj.SpiritStonesHigh > 0)
                    SpiritStonesHigh = roleAssetsObj.SpiritStonesHigh + assetsServer.SpiritStonesHigh;
                SpiritStonesMedium = roleAssetsObj.SpiritStonesMedium;
                if (roleAssetsObj.SpiritStonesMedium > 0)
                    SpiritStonesMedium = roleAssetsObj.SpiritStonesMedium + assetsServer.SpiritStonesMedium;
                XianYu = roleAssetsObj.XianYu + assetsServer.XianYu;
                if (roleAssetsObj.XianYu > 0)
                    XianYu = roleAssetsObj.XianYu + assetsServer.XianYu;
                Singleton<NHManager>.Instance.Update<RoleAssets>(new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesHigh = SpiritStonesHigh, SpiritStonesLow = SpiritStonesLow, SpiritStonesMedium = SpiritStonesMedium, XianYu = XianYu });
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
