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
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>SyncRoleAssetsHandler\n" + roleJson + "\n SyncRoleAssetsHandler >>>>>>>>>>>>>>>>>>>>>>");
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            Owner. ResponseData.Clear();
            Owner. OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
                var result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);

                {
                    if (result == null)
                    {
                        Singleton<NHManager>.Instance.Add(new RoleAssets()
                        {
                            RoleID = roleObj.RoleID,
                            SpiritStonesHigh = 100,
                            SpiritStonesLow = 100,
                            SpiritStonesMedium = 100,
                            XianYu = 50
                        });
                        result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);
                    }
                }
                string roleAssetsJson = Utility.ToJson(result);
                AscensionServer._Log.Info("SyncRoleAssetsHandler\n" + roleAssetsJson + "\nSyncRoleAssetsHandler");
               Owner. ResponseData.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
               Owner. OpResponse.Parameters =Owner. ResponseData;
                Owner. OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
            {
               Owner. OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner. OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
