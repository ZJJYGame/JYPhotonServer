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
    public class RoleStatus
    {
        public virtual int RoleId { get; set; }
        public virtual int RoleMaxHP { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMaxMP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual byte RoleJingXue { get; set; }
        public virtual int RoleAttackDamage { get; set; }
        public virtual int RoleResistanceDamage { get; set; }
        public virtual int RoleAttackPower { get; set; }
        public virtual int RoleResistancePower { get; set; }
        public virtual int RoleSpeedAttack { get; set; }
        public virtual int RoleShenHunDamage { get; set; }
        public virtual int RoleShenHunResistance { get; set; }
        public virtual byte RoleCrit { get; set; }
        public virtual byte RoleCritResistance { get; set; }
        public override bool Equals(object obj)
        {
            RoleStatus other = obj as RoleStatus;
            if (other == null) return false;
            if (this.RoleId == other.RoleId && this.RoleHP == other.RoleHP && this.RoleMP == other.RoleMP
                && this.RoleJingXue == other.RoleJingXue && this.RoleAttackDamage == other.RoleAttackDamage &&
                this.RoleResistanceDamage == other.RoleResistanceDamage && this.RoleAttackPower == other.RoleAttackPower &&
                this.RoleResistancePower == other.RoleResistancePower && this.RoleSpeedAttack == other.RoleSpeedAttack &&
                this.RoleShenHunDamage == other.RoleShenHunDamage && this.RoleShenHunResistance == other.RoleShenHunResistance &&
                this.RoleCrit == other.RoleCrit && this.RoleCritResistance == other.RoleCritResistance)
                return true;
            else return false;
        }
        public override string ToString()
        {
            List<int> info = new List<int>();
            info.Add(RoleId);
            info.Add(RoleMaxHP);
            info.Add(RoleHP);
            info.Add(RoleMaxMP);
            info.Add(RoleMP);
            info.Add(RoleJingXue);
            info.Add(RoleAttackDamage);
            info.Add(RoleResistanceDamage);
            info.Add(RoleAttackPower);
            info.Add(RoleResistancePower);
            info.Add(RoleSpeedAttack);
            info.Add(RoleShenHunDamage);
            info.Add(RoleShenHunResistance);
            info.Add(RoleCrit);
            info.Add(RoleCritResistance);
            return Utility.ToJson(info);
        }
    }
}
