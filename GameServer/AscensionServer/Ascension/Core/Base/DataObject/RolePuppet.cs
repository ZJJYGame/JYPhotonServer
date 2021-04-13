using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RolePuppet
    {
        public virtual int RoleID { get; set; }
        public virtual string PuppetDict { get; set; }
        public virtual int IsBattle { get; set; }

        public RolePuppet()
        {
            PuppetDict ="{}";
            IsBattle = 0;
        }
    }
}
