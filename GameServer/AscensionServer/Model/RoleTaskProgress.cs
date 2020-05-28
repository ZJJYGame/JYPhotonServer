using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
   public class RoleTaskProgress
    {
        public virtual int RoleID { get; set; }
        public virtual string RoleTaskID { get; set; }
        public virtual string RoleTaskState { get; set; }
    }
}
