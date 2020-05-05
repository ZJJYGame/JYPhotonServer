/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色属性 
*/

namespace AscensionServer.Model
{
    public class RoleStatus
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
