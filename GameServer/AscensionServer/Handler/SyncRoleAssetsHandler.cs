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
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
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
                ResponseData.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
                OpResponse.Parameters = ResponseData;
                OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
            {
                OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
