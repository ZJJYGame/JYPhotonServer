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
    public class GetTeamSubHandler : SyncTeamSubHandler
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

            if (exist)
            {
                switch (RoleObj.teamInstructions)
                {
                    case RoleDTO.TeamInstructions.CreateTeam:
                        if (!AscensionServer.Instance._playerIdToTeamIdDict.ContainsKey(RoleObj.RoleID))
                            AscensionServer.Instance.CreateTeam(RoleObj, new int[] { 0, 99 });
                        break;
                    case RoleDTO.TeamInstructions.JoinTeam:
                        break;
                    case RoleDTO.TeamInstructions.ApplyTeam:
                        AscensionServer.Instance.ApplyJoinTeam(RoleObj, 1001);
                        break;
                    default:
                        break;
                }
                Owner.ResponseData.Add((byte)ParameterCode.RoleTeam, Utility.Json.ToJson(AscensionServer.Instance._teamTOModel));
                Owner.ResponseData.Add((byte)ParameterCode.Role, Utility.Json.ToJson(AscensionServer.Instance._playerIdToTeamIdDict));
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}
