using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class RoleRing
    {
        //TODO待确认储物戒指
        public virtual int RoleID { get; set; }
        public virtual int RingID { get; set; }
    }
}
