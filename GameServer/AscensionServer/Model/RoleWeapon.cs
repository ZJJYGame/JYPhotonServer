using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleWeapon:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string WeaponIDAttay { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            WeaponIDAttay = null;
        }
    }
}
