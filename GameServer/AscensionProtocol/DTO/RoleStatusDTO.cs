using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleStatusDTO:ProtocolDTO
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
        public virtual short RoleShenhun { get; set; }
        public virtual short RoleMaxShenhun { get; set; }
        public virtual int RoleShenHunDamage { get; set; }
        public virtual int RoleShenHunResistance { get; set; }
        public virtual byte RoleCrit { get; set; }
        public virtual byte RoleCritResistance { get; set; }

        public override bool Equals(object obj)
        {
            RoleStatusDTO other = obj as RoleStatusDTO;
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
            string str = "";
            str += "roleid:" + RoleId + ">>roleHp" + RoleHP + ">>roleMP" + RoleMP+">>roleShenhun"+RoleShenhun+">>roleJingxue"+RoleJingXue;
            return str;
        }
    }
}