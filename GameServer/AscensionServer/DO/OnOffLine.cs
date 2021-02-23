using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class OnOffLine :DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual int GongFaExp { get; set; }
        public virtual int MiShuExp { get; set; }
        public virtual int MsGfID { get; set; }
        public virtual int ExpType { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            GongFaExp = 0;
            MiShuExp = 0;
            MsGfID = 0;
        }
    }
}


