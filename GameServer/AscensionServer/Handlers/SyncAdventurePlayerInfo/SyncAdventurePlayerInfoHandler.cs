using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Photon.SocketServer;
using System;
using AscensionServer.Model;

namespace AscensionServer
{
    public class SyncAdventurePlayerInfoHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncAdventurePlayerInfo; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var playerJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var playerObj = Utility.Json.ToObject<RoleDTO>(playerJson);
            Utility.Debug.LogInfo("yzqData收到的玩家信息" + playerJson);
            NHCriteria nHCriteriarole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", playerObj.RoleID);



            var roleallianceTemp = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriarole);
            NHCriteria nHCriteriaalliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", roleallianceTemp.AllianceID);
            var allianceStatusTemp = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaalliance);
            if (allianceStatusTemp==null)
            {
                allianceStatusTemp = new AllianceStatus();
            }

            var roleTemp = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriarole);
            var roleBottleneckTemp = NHibernateQuerier.CriteriaSelect<Bottleneck>(nHCriteriarole);
            if (roleBottleneckTemp == null)
            {
                roleBottleneckTemp = new Bottleneck();
            }
            var roleStatusTemp = NHibernateQuerier.CriteriaSelect<RoleStatus>(nHCriteriarole);

            AdventurePlayerDTO adventurePlayerDTO = new AdventurePlayerDTO()
            {
                RoleName = roleTemp.RoleName,
                RoleFaction = roleTemp.RoleFaction,
                RoleGender = roleTemp.RoleGender,
                RoleLevel = roleTemp.RoleLevel,
                RoleHP = roleStatusTemp.RoleHP,
                RoleMP = roleStatusTemp.RoleMP,
                RoleJingXue = roleStatusTemp.RoleJingXue,
                RoleAttackDamage = roleStatusTemp.RoleAttackDamage,
                RoleResistanceDamage = roleStatusTemp.RoleResistanceDamage,
                RoleAttackPower = roleStatusTemp.RoleAttackPower,
                RoleResistancePower = roleStatusTemp.RoleResistancePower,
                RoleSpeedAttack = roleStatusTemp.RoleSpeedAttack,
                RoleShenhun = roleStatusTemp.RoleShenhun,
                RoleShenHunDamage = roleStatusTemp.RoleShenHunDamage,
                RoleShenHunResistance = roleStatusTemp.RoleShenHunResistance,
                RoleCrit = roleStatusTemp.RoleCrit,
                RoleCritResistance = roleStatusTemp.RoleCritResistance,
                RoleVileSpawn = roleStatusTemp.RoleVileSpawn,
                RoleVitality = roleStatusTemp.RoleVitality,
                RoleKillingIntent = roleBottleneckTemp.CraryVaule,
                RoleDormant = roleStatusTemp.RoleDormant,
                RoleID = roleTemp.RoleID,
                RoleAllianceName = allianceStatusTemp.AllianceName
            };
            operationResponse.ReturnCode = (short)ReturnCode.Success;
            operationResponse.DebugMessage = Utility.Json.ToJson(adventurePlayerDTO);

            GameManager.ReferencePoolManager.Despawns(nHCriteriaalliance, nHCriteriarole);

            return operationResponse;
        }
    }
}
