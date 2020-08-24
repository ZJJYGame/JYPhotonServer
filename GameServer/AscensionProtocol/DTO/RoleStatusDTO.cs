﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleStatusDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int RoleMaxHP { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMaxMP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual int RoleJingXue { get; set; }
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
        public virtual byte RoleCrit { get; set; }
        public virtual byte RoleCritResistance { get; set; }
        public virtual int RoleDormant { get; set; }
        public virtual int RoleVileSpawn { get; set; }//人物业障
        public virtual int RoleVitality { get; set; }//人物活力
        public virtual int RoleKillingIntent  { get; set; }//人物煞气

        public override bool Equals(object obj)
        {
            RoleStatusDTO other = obj as RoleStatusDTO;
            if (other == null) return false;
            if (this.RoleID == other.RoleID && this.RoleHP == other.RoleHP && this.RoleMP == other.RoleMP
                && this.RoleJingXue == other.RoleJingXue && this.RoleAttackDamage == other.RoleAttackDamage &&
                this.RoleResistanceDamage == other.RoleResistanceDamage && this.RoleAttackPower == other.RoleAttackPower &&
                this.RoleResistancePower == other.RoleResistancePower && this.RoleSpeedAttack == other.RoleSpeedAttack &&
                this.RoleShenHunDamage == other.RoleShenHunDamage && this.RoleShenHunResistance == other.RoleShenHunResistance &&
                this.RoleCrit == other.RoleCrit && this.RoleCritResistance == other.RoleCritResistance&&this.RoleDormant==other.RoleDormant&&this.RoleVileSpawn==other.RoleVileSpawn&&this.RoleVitality==other.RoleVitality&&this.RoleKillingIntent==other.RoleKillingIntent)
                return true;
            else return false;
        }
        public override string ToString()
        {
            string str = "";
            str += "roleid:" + RoleID + ">>roleHp" + RoleHP + ">>roleMP" + RoleMP + ">>roleShenhun" + RoleShenhun + ">>roleJingxue" + RoleJingXue;
            return str;
        }
        public override void Clear()
        {
            RoleID = -1;
            RoleMaxHP = 0;
            RoleHP = 0;
            RoleMaxMP = 0;
            RoleMP = 0;
            RoleJingXue = 0;
            RoleMaxJingXue = 0;
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
        }
    }
}