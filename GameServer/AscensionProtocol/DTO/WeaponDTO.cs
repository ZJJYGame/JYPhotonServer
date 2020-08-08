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
       public virtual int RoleID { set; get; }
        public virtual Dictionary<int, WeaponStatusDTO> WeaponStatusDict { set; get; }
        public virtual Dictionary<int, int> Weaponindex { set; get; }//武器全局id为key,新增id下标为value

        public override void Clear()
        {
            RoleID = -1;
            WeaponStatusDict = null;
            Weaponindex =null;

        }

        public class WeaponStatusDTO
        {
            public virtual int WeaponID { get; set; }
            public virtual int WeaponPart { get; set; }
            public virtual int WeaponLevel { get; set; }
            public virtual int WeaponHP { get; set; }
            public virtual int WeaponSpeed { get; set; }
            public virtual int WeaponAttackDamage { get; set; }
            public virtual int WeaponResistanceDamage { get; set; }
            public virtual int WeaponAttackPower { get; set; }
            public virtual int WeaponResistancePower { get; set; }
            public virtual List<int> WeaponSkillLIst { get; set; }
        }
    }
}
