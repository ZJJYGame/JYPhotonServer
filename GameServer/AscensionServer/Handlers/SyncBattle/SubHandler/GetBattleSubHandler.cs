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
    public class GetBattleSubHandler : SyncBattleSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var battleData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role) as string;
            var battleTransferData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleBattle) as string;
            Utility.Debug.LogInfo(">>>>>进入战斗请求" + battleData + ">>>>>>>>>>>>>");
            Utility.Debug.LogInfo(">>>>>战斗请求传输数据" + battleTransferData + ">>>>>>>>>>>>>");
            var RoleObj = Utility.Json.ToObject<RoleDTO>(battleData);
            var battleTransferObj = Utility.Json.ToObject<BattleTransferDTO>(battleTransferData);
            NHCriteria nHCriteriaRoleID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                switch (RoleObj.SendBattleCmd)
                {
                    case BattleCmd.Init:
                        if (!GameEntry.ServerBattleManager.TeamIdToBattleInit.ContainsKey(RoleObj.RoleID))
                            GameEntry.ServerBattleManager.EntryBattle(RoleObj.BattleInitDTO);
                        else
                        {
                            GameEntry.ServerBattleManager.TeamIdToBattleInit.Remove(RoleObj.RoleID);
                            GameEntry.ServerBattleManager.TeamIdToBattleInitData.Remove(RoleObj.RoleID);
                            GameEntry.ServerBattleManager.EntryBattle(RoleObj.BattleInitDTO);
                        }
                        break;
                    case BattleCmd.Prepare:
                        GameEntry.ServerBattleManager.PrepareBattle(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId);
                        break;
                    case BattleCmd.PropsInstruction:
                        GameEntry.ServerBattleManager.BattlePropsInstrution(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.SkillInstruction:
                        GameEntry.ServerBattleManager.BattleStart( RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.RunAwayInstruction:
                        GameEntry.ServerBattleManager.BattleRunAway(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.MagicWeapon:
                        GameEntry.ServerBattleManager.BattleMagicWeapen(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.CatchPet:
                        GameEntry.ServerBattleManager.BattleCatchPet(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.SummonPet:
                        GameEntry.ServerBattleManager.BattleSummonPet(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.Defend:
                        GameEntry.ServerBattleManager.BattleDefend(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                }
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            CosmosEntry.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}


