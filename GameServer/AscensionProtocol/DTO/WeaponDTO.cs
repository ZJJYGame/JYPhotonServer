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
        public virtual int WeaponID { set; get; }
        public virtual Dictionary<int, List<int>> WeaponStatusDict { set; get; }
        public virtual Dictionary<int, int> Weaponindex { set; get; }//武器全局id为key,新增id下标为value

        public override void Clear()
        {
            RoleID = -1;
            WeaponStatusDict = null;
            Weaponindex =null;

        }      
    }

}
