﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* Time  2020.4.28
 * Content : 角色表
 * Host : xianrenZhang 
 */
namespace AscensionServer.Model
{
    public class Role
    {
        public virtual int RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual byte RoleGender { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual byte RoleJingXue { get; set; }
        public virtual int RoleAttackDamage { get; set; }
        public virtual int RoleResistanceDamage { get; set; }
        public virtual int RoleAttackPower { get; set; }
        public virtual int RoleResistancePower { get; set; }
        public virtual int RoleSpeedAttack { get; set; }
        public virtual int RoleSpeedMove { get; set; }
        public virtual int RoleShenHunDamage { get; set; }
        public virtual int RoleShenHunResistance { get; set; }
        public virtual byte RoleCrit { get; set; }
        public virtual byte RoleCritResistance { get; set; }
        public virtual string RoleUserAccount { get; set; }
        public virtual ISet<RoleGongFa> RoleGongFaList { get; set; }
        public virtual ISet<RoleLevel> RoleLevelList { get; set; }
    }
}
