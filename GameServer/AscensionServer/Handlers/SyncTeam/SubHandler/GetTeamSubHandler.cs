using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;
using Cosmos;
namespace AscensionServer
{
    public class GetTeamSubHandler:SyncTeamSubHandler
    {
        public override void OnInitialization() => SubOpCode = SubOperationCode.Get;
       
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var teamData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            Utility.Debug.LogInfo(">>>>>创建队伍" + teamData + ">>>>>>>>>>>>>");
            var RoleObj = Utility.Json.ToObject<RoleDTO>(teamData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            if (!exist)
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Empty;
            else
            {
                if (!AscensionServer.Instance._playerIdToTeamIdDict.ContainsKey(RoleObj.RoleID))
                {
                    AscensionServer.Instance.CreateTeam(RoleObj, new int[] { 0 ,99 });
                    Owner.ResponseData.Add((byte)ParameterCode.RoleTeam, AscensionServer.Instance._teamTOModel);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}
