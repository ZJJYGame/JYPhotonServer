using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class WeaponDTO : DataTransferObject
    {
        public virtual int ID { get; set; }
        public virtual int WeaponID { get; set; }
        public virtual int WeaponPart { get; set; }
        public virtual int WeaponLevel { get; set; }
        public virtual int WeaponHP { get; set; }
        public virtual int WeaponSpeed { get; set; }
        public virtual int WeaponAttackDamage { get; set; }
        public virtual int WeaponResistanceDamage { get; set; }
        public virtual int WeaponAttackPower { get; set; }
        public virtual int WeaponResistancePower { get; set; }

        public override void Clear()
        {
            ID = -1;
            WeaponID = 0;
            WeaponPart = 0;
            WeaponLevel = 0;
            WeaponHP = 0;
            WeaponSpeed = 0;
            WeaponAttackDamage = 0;
            WeaponResistanceDamage = 0;
            WeaponAttackPower = 0;
            WeaponResistancePower = 0;
        }
    }
}
