using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleWeaponDTO : DataTransferObject
    {
       public virtual int RoleID { set; get; }
        public virtual int WeaponID { set; get; }
        public virtual Dictionary<int, WeaponDTO> WeaponStatusDict { set; get; }
        public virtual Dictionary<int, int> Weaponindex { set; get; }//武器全局id为key,新增id下标为value
        public virtual Dictionary<int, WeaponDTO> MagicStatusDict { set; get; }
        public virtual Dictionary<int, int> Magicindex { set; get; }//武器全局id为key,新增id下标为value
        public override void Clear()
        {
            RoleID = -1;
            WeaponStatusDict = null;
            Weaponindex =null;
            MagicStatusDict = null;
            Magicindex = null;
        }      
    }
    [Serializable]
    public class WeaponDTO : DataTransferObject
    {
        public virtual int WeaponQuality { set; get; }
        public virtual int NeedLevelID { set; get; }
        public virtual List<int> WeaponAttribute { set; get; }
        public virtual int WeaponDurable { set; get; }
        public virtual List<int> WeaponSkill { set; get; }

        public override void Clear()
        {
            WeaponQuality = 1;
            NeedLevelID = 1;
            WeaponAttribute = new List<int>();
            WeaponDurable = 1;
            WeaponSkill = new List<int>();
        }
    }
}
