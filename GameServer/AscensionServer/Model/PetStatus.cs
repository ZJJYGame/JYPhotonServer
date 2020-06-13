using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class PetStatus
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
        public virtual string PetTalent { get; set; }
        public virtual short PetMaxShenhun { get; set; }
        public virtual short PetShenhun { get; set; }
        public virtual int PetShenhunDamage { get; set; }
        public virtual int PetShenhunResistance { get; set; }
    }
}
