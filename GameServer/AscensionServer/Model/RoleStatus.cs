﻿/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色属性 
*/
using System.Collections.Generic;
using System;
namespace AscensionServer.Model
{
    [Serializable]
    public class RoleStatus
    {
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
    }
}
