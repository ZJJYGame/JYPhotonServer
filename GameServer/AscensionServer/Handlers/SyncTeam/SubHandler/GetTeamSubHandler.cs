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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var teamData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            Utility.Debug.LogInfo(">>>>>创建队伍" + teamData + ">>>>>>>>>>>>>");
            var RoleObj = Utility.Json.ToObject<RoleDTO>(teamData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
  
            if (exist)
            {
                switch (RoleObj.teamInstructions)
                {
                    case RoleDTO.TeamInstructions.CreateTeam:
                        if (!AscensionServer.Instance._playerIdToTeamIdDict.ContainsKey(RoleObj.RoleID))
                            AscensionServer.Instance.CreateTeam(RoleObj, new int[] { 0, 99 });
                        break;
                    case RoleDTO.TeamInstructions.JoinTeam:
                        AscensionServer.Instance.JoinTeam(RoleObj,RoleObj.teamDTO.TeamId);
                        break;
                    case RoleDTO.TeamInstructions.ApplyTeam:
                        AscensionServer.Instance.ApplyJoinTeam(RoleObj, RoleObj.teamDTO.TeamId);
                        break;
                    case RoleDTO.TeamInstructions.RefusedTeam:
                        AscensionServer.Instance.RefusedApplyTeam(RoleObj, RoleObj.teamDTO.TeamId);
                        break;
                    default:
                        break;
                }
                subResponseParameters.Add((byte)ParameterCode.RoleTeam, Utility.Json.ToJson(AscensionServer.Instance._teamTOModel));
                subResponseParameters.Add((byte)ParameterCode.Role, Utility.Json.ToJson(AscensionServer.Instance._playerIdToTeamIdDict));
                operationResponse.Parameters = subResponseParameters;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}
