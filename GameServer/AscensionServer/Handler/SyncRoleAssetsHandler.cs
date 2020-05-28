using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using System.Diagnostics;
namespace AscensionServer
{
    public class SyncRoleAssetsHandler : BaseHandler
    {
        public SyncRoleAssetsHandler()
        {
            OpCode = OperationCode.SyncRoleAssets;
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer.log.Info(">>>>>>>>>>>>>SyncRoleAssetsHandler\n" + roleJson+ "\n SyncRoleAssetsHandler >>>>>>>>>>>>>>>>>>>>>>");
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
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
                AscensionServer.log.Info("SyncRoleAssetsHandler\n" + roleAssetsJson + "\nSyncRoleAssetsHandler");
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
                response.Parameters = data;
                response.ReturnCode = (byte)ReturnCode.Success;
            }
            else
            {
                response.ReturnCode = (byte)ReturnCode.Fail;
            }
            peer.SendOperationResponse(response, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
