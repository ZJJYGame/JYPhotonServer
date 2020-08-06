using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public  class RoleWeapon:DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual string WeaponIDAttay { get; set; }//部位id为key,武器id为value
        public override void Clear()
        {
            RoleID = -1;
            WeaponIDAttay = null;
        }
    }
}
