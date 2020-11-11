using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 角色id
/// 角色最大血量
/// 角色血量
/// 角色最大灵力
/// 角色灵力
/// 角色精血
/// 角色物理伤害
/// 角色物理防御
/// 角色攻击力
/// 角色防御力
/// 角色攻击速度
/// 角色神魂
/// 角色最大神魂
/// 角色神魂攻击
/// 角色神魂防御
/// 角色暴击
/// 角色暴击防御
/// 角色隐匿值
/// </summary>

namespace AscensionServer
{
    public class AddRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var RoleStatusData = Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleStatus) as string;
            Utility.Debug.LogInfo(">>>>>添加自己的一些属性" + RoleStatusData + ">>>>>>>>>>>>>");
            var roleObj = Utility.Json.ToObject<RoleStatus>(RoleStatusData);
            NHCriteria nHCriteriaRoleId = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleStatus>(nHCriteriaRoleId);
            if (exist)
            {
                var  roleStatusSever =  NHibernateQuerier.CriteriaSelect<RoleStatus>(nHCriteriaRoleId);
                
                if (roleObj.RoleMaxHP != 0 )
                {
                    roleStatusSever.RoleMaxHP += roleObj.RoleMaxHP;
                    //if ((roleStatusSever.RoleHP + roleObj.RoleHP) < 0)
                    //    roleStatusSever.RoleHP = 0;
                }

                if (roleObj.RoleMP != 0)
                {
                    roleStatusSever.RoleMP += roleObj.RoleMP;
                    //if ((roleStatusSever.RoleMP + roleObj.RoleMP) <0)
                    //    roleStatusSever.RoleMP = 0;
                }

                if (roleObj.PhysicalCritDamage != 0 )
                {
                    roleStatusSever.PhysicalCritDamage += roleObj.PhysicalCritDamage;
                    //if ((roleStatusSever.RoleAttackDamage + roleObj.RoleAttackDamage) < 0)
                    //    roleStatusSever.RoleAttackDamage = 0;
                }


                if (roleObj.DefendPhysical != 0)
                {
                    roleStatusSever.DefendPhysical += roleObj.DefendPhysical;
                    //if ((roleStatusSever.RoleResistanceDamage + roleObj.RoleResistanceDamage) < 0)
                    //    roleStatusSever.RoleResistanceDamage = 0;
                }

                if (roleObj.AttackPower != 0)
                {
                    roleStatusSever.AttackPower += roleObj.AttackPower;
                    //if ((roleStatusSever.RoleAttackPower + roleObj.RoleAttackPower) < 0)
                    //    roleStatusSever.RoleAttackPower = 0;
                }

                if (roleObj.DefendPower != 0 )
                {
                    roleStatusSever.DefendPower += roleObj.DefendPower;
                    //if ((roleStatusSever.RoleResistancePower + roleObj.RoleResistancePower) < 0)
                    //    roleStatusSever.RoleResistancePower = 0;
                }

                if (roleObj.AttackSpeed != 0)
                {
                    roleStatusSever.AttackSpeed += roleObj.AttackSpeed;
                    //if ((roleStatusSever.RoleSpeedAttack + roleObj.RoleSpeedAttack) < 0)
                    //    roleStatusSever.RoleSpeedAttack = 0;
                }


                NHibernateQuerier.Update(new RoleStatus()
                {
                    RoleID = roleObj.RoleID,
                    RoleHP = roleStatusSever.RoleHP,
                    RoleMaxMP = roleStatusSever.RoleMP,
                    RoleMaxHP = roleStatusSever.RoleMaxHP,            
                    RoleMP = roleStatusSever.RoleMP,                   
                });
                operationResponse.Parameters = subResponseParameters;
                operationResponse.ReturnCode = (short)ReturnCode.Success;
            }else operationResponse.ReturnCode = (short)ReturnCode.Fail;
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleId);
            return operationResponse;
        }

        #region     计算数据
        /*
         if (roleObj.RoleSpeedAttack != 0 && roleStatusSever.RoleSpeedAttack > 0)
                    roleStatusSever.RoleSpeedAttack += roleObj.RoleSpeedAttack;

                if (roleObj.RoleShenHunDamage != 0 && roleStatusSever.RoleShenHunDamage > 0)
                    roleStatusSever.RoleShenHunDamage += roleObj.RoleShenHunDamage;

                if (roleObj.RoleShenHunResistance != 0 && roleStatusSever.RoleShenHunResistance > 0)
                    roleStatusSever.RoleShenHunResistance += roleObj.RoleShenHunResistance;

                if (roleObj.RoleCrit != 0 && roleStatusSever.RoleCrit > 0)
                    roleStatusSever.RoleCrit += roleObj.RoleCrit;

                if (roleObj.RoleCritResistance != 0 && roleStatusSever.RoleCritResistance > 0)
                    roleStatusSever.RoleCritResistance += roleObj.RoleCritResistance;*/
        #endregion
    }
}
