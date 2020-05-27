#define CreateNewRoleAssets
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
namespace AscensionServer.Handler
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
            var roleObj = Utility.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            NHCriteria nHCriteriaRoleName = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleObj.RoleName);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID,nHCriteriaRoleName);
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            if (exist)
            {
                var result = Singleton<NHManager>.Instance.CriteriaGet<RoleAssets>(nHCriteriaRoleID);
                CreateNewRoleAssets(result,roleObj);
                string roleAssetsJson = Utility.ToJson(result);
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
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID, nHCriteriaRoleName);
        }
        //[Conditional("CreateNewRoleAssets")]
        void CreateNewRoleAssets(RoleAssets ra,Role role)
        {
            if (ra == null)
                Singleton<NHManager>.Instance.Add(new RoleAssets() { RoleID = role.RoleID,
                    SpiritStonesLow =100 ,SpiritStonesMedium=100,SpiritStonesHigh = 100
                });
        }
    }
}
