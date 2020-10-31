/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色属性 
*/
using System.Collections.Generic;
using System;
namespace AscensionServer.Model
{
    [Serializable]
    public class RoleStatus:DataObject
    {
        public RoleStatus()
        {

            RoleMaxHP = 100;
            RoleHP = 488;
            RoleMaxMP = 100;
            RoleMP = 300;
            RoleJingXue = 0;
            RoleAttackDamage = 40;
            RoleResistanceDamage = 40;
            RoleAttackPower = 30;
            RoleResistancePower = 40;
            RoleSpeedAttack = 40;
            RoleShenhun = 320;
            RoleMaxShenhun = 60;
            RoleShenHunDamage = 10;
            RoleShenHunResistance = 10;
            RoleCrit = 0;
            RoleCritResistance = 0;
            RoleDormant = 10;
            RoleKillingIntent = 0;
            RoleVitality = 0;
            RoleVileSpawn = 0;
            RoleMaxJingXue = 0;
            RoleMoveSpeed = 3;
        }
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
        public virtual int RoleID { get; set; }
        public virtual int RoleMaxHP { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMaxMP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual short RoleJingXue { get; set; }
        public virtual int RoleMaxJingXue { get; set; }
        public virtual int RoleAttackDamage { get; set; }
        public virtual int RoleResistanceDamage { get; set; }
        public virtual int RoleAttackPower { get; set; }
        public virtual int RoleResistancePower { get; set; }
        public virtual int RoleSpeedAttack { get; set; }
        public virtual short RoleShenhun { get; set; }
        public virtual short RoleMaxShenhun { get; set; }
        public virtual int RoleShenHunDamage { get; set; }
        public virtual int RoleShenHunResistance { get; set; }
        public virtual short RoleCrit { get; set; }
        public virtual short RoleCritResistance { get; set; }
        public virtual int RoleDormant { get; set; }
        public virtual int RoleVileSpawn { get; set; }//人物业障
        public virtual int RoleVitality { get; set; }//人物活力
        public virtual int RoleKillingIntent { get; set; }//人物煞气
        public virtual int RoleMoveSpeed { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            RoleMaxHP = 0;
            RoleHP = 0;
            RoleMaxMP = 0;
            RoleMP = 0;
            RoleJingXue = 0;
            RoleAttackDamage = 0;
            RoleResistanceDamage = 0;
            RoleAttackPower = 0;
            RoleResistancePower = 0;
            RoleSpeedAttack = 0;
            RoleShenhun = 0;
            RoleMaxShenhun = 0;
            RoleShenHunDamage = 0;
            RoleShenHunResistance = 0;
            RoleCrit = 0;
            RoleCritResistance = 0;
            RoleDormant = 0;
            RoleKillingIntent = 0;
            RoleVitality = 0;
            RoleVileSpawn = 0;
            RoleMoveSpeed = 0;
        }
    }
}
