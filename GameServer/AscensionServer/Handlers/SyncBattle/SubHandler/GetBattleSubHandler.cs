﻿using System;
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
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", RoleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                switch (RoleObj.SendBattleCmd)
                {
                    case BattleCmd.Init:
                        if (!GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit.ContainsKey(RoleObj.RoleID))
                            GameManager.CustomeModule<ServerBattleManager>().EntryBattle(RoleObj.BattleInitDTO);
                        else
                        {
                            GameManager.CustomeModule<ServerBattleManager>()._teamIdToBattleInit.Remove(RoleObj.RoleID);
                            GameManager.CustomeModule<ServerBattleManager>().EntryBattle(RoleObj.BattleInitDTO);
                        }
                        break;
                    case BattleCmd.Prepare:
                        GameManager.CustomeModule<ServerBattleManager>().PrepareBattle(RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId);
                        break;
                    case BattleCmd.PropsInstruction:
                        break;
                    case BattleCmd.SkillInstruction:
                        GameManager.CustomeModule<ServerBattleManager>().BattleStart(RoleObj.SendBattleCmd, RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    case BattleCmd.RunAwayInstruction:
                        GameManager.CustomeModule<ServerBattleManager>().BattleRunAway(RoleObj.SendBattleCmd, RoleObj.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID, RoleObj.BattleInitDTO.RoomId, battleTransferObj);
                        break;
                    default:
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
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
            return operationResponse;
        }
    }
}
