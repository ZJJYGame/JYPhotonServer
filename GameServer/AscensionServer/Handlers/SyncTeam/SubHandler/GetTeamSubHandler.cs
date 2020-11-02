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
                    case TeamInstructions.Init:
                        GameManager.CustomeModule<ServerTeamManager>().ServerToClientInit(RoleObj.RoleID);
                        break;
                    case TeamInstructions.CreateTeam:
                        if (!GameManager.CustomeModule<ServerTeamManager>()._playerIdToTeamIdDict.ContainsKey(RoleObj.RoleID))
                            GameManager.CustomeModule<ServerTeamManager>().CreateTeam(RoleObj, new int[] { 0, 99 });
                        break;
                    case TeamInstructions.JoinTeam:
                        GameManager.CustomeModule<ServerTeamManager>().JoinTeam(RoleObj,RoleObj.teamDTO.TeamId);
                        break;
                    case TeamInstructions.ApplyTeam:
                        GameManager.CustomeModule<ServerTeamManager>().ApplyJoinTeam(RoleObj, RoleObj.teamDTO.TeamId);
                        break;
                    case TeamInstructions.RefusedTeam:
                        GameManager.CustomeModule<ServerTeamManager>().RefusedApplyTeam(RoleObj, RoleObj.teamDTO.TeamId);
                        break;
                    default:
                        break;
                }
                
                SetResponseParamters(() =>
                {
                    //subResponseParameters.Add((byte)ParameterCode.RoleTeam, Utility.Json.ToJson(GameManager.CustomeModule<ServerTeamManager>()._teamTOModel));
                    //subResponseParameters.Add((byte)ParameterCode.Role, Utility.Json.ToJson(GameManager.CustomeModule<ServerTeamManager>()._playerIdToTeamIdDict));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;

                });
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}
