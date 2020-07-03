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
        }
        public virtual int RoleID { get; set; }
        public virtual int RoleMaxHP { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMaxMP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual short RoleJingXue { get; set; }
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
        }
    }
}
