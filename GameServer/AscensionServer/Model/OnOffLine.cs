using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class OnOffLine 
    {
        public virtual int RoleID { get; set; }
        public virtual string OnLineTime { get; set; }
        public virtual string OffLineTime { get; set; }
    }
}
