using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleEquip:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string Equips { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            Equips = null;
        }
    }
}
