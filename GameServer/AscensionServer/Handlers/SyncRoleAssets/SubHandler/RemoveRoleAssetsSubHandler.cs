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
    public class RemoveRoleAssetsSubHandler : SyncRoleAssetsSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roleAssetsJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAssets));

            var roleAssetsObj = Utility.Json.ToObject<RoleAssets>(roleAssetsJson);
            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleAssetsObj.RoleID);
            bool roleExist = ConcurrentSingleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleAssets>(nHCriteriaRoleID);
            long SpiritStonesLow = 0;
            long SpiritStonesHigh = 0;
            long SpiritStonesMedium = 0;
            long XianYu = 0;
            if (roleExist && roleAssetsExist)
            {
               
                var assetsServer = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);
                SpiritStonesLow = assetsServer.SpiritStonesLow;
                if (roleAssetsObj.SpiritStonesLow > 0&& roleAssetsObj.SpiritStonesLow <=assetsServer.SpiritStonesLow)
                    SpiritStonesLow = assetsServer.SpiritStonesLow- roleAssetsObj.SpiritStonesLow ;
                AscensionServer._Log.Info(">>>>>>>>>>>>>減少的資產：" + SpiritStonesLow + ">>>>>>>>>>>>>>>>>>>>>>");
                if (roleAssetsObj.XianYu > 0&& roleAssetsObj.XianYu <= assetsServer.XianYu)
                    XianYu = assetsServer.XianYu- roleAssetsObj.XianYu ;
                ConcurrentSingleton<NHManager>.Instance.Update<RoleAssets>(new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesLow = SpiritStonesLow, XianYu = XianYu });
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            AscensionServer._Log.Info(">>>>>>>>>>>>>發送囘u去：" + roleAssetsJson + ">>>>>>>>>>>>>>>>>>>>>>");
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
