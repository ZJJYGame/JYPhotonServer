using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    public class Role_Status
    {
        public virtual int RoleId { get; set; }
        public virtual byte RoleGender { get; set; }
        public virtual int RoleHP { get; set; }
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
    }
}
