using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class OffLineTime:Model
    {
        public virtual int RoleID { get; set; }
        public virtual string OffTime { get; set; }

        public override void Clear()
        {
            RoleID = -1;
            OffTime = null;
        }
    }
}
