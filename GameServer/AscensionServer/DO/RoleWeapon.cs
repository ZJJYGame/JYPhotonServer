using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Weapon:DataObject
    {
        public Weapon()
        {
            WeaponStatusDict ="{}";
            Weaponindex = "{}";
        }

        public virtual int RoleID { set; get; }
        public virtual string WeaponStatusDict { set; get; }
        public virtual string Weaponindex { set; get; }//武器全局id为key,新增id下标为value
        public virtual string MagicStatusDict { set; get; }
        public virtual string Magicindex { set; get; }
        
        public override void Clear()
        {
            RoleID = -1;
            WeaponStatusDict = null;
            Weaponindex = null;
        }
    }
}


