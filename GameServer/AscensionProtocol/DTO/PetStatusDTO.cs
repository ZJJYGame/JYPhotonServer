using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PetStatusDTO: DataTransferObject
    {
        public virtual int PetID { get; set; }
        public virtual short PetSpeed { get; set; }
        public virtual short PetAttackDamage { get; set; }
        public virtual short PetResistanceAttack { get; set; }
        public virtual short PetAbilityPower { get; set; }
        public virtual short PetResistancePower { get; set; }
        public virtual int PetMaxHP { get; set; }
        public virtual int PetHP { get; set; }
        public virtual int PetMaxMP { get; set; }
        public virtual int PetMP { get; set; }
        public virtual int PetTalent { get; set; }
        public virtual short PetMaxShenhun { get; set; }
        public virtual short PetShenhun { get; set; }
        public virtual int PetShenhunDamage { get; set; }
        public virtual int PetShenhunResistance { get; set; }
        public override void Clear()
        {
            PetID = 0;
            PetSpeed = 0;
            PetAttackDamage = 0;
            PetResistanceAttack = 0;
            PetAbilityPower = 0;
            PetResistancePower = 0;
            PetMaxHP = 0;
            PetHP = 0;
            PetMaxMP = 0;
            PetMP = 0;
            PetTalent = 0;
            PetMaxShenhun = 0;
            PetShenhun = 0;
            PetShenhunDamage = 0;
            PetShenhunResistance = 0;
        }
    }
}
