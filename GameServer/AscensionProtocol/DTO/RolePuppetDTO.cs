using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public  class RolePuppetDTO: DataTransferObject
    {
        public virtual int RoleID { set; get; }
        public virtual bool IsBattle { set; get; }
        public virtual Dictionary<int,int> PuppetDict { set; get; }

        public RolePuppetDTO()
        {
            RoleID = 0;
            IsBattle = false;
            PuppetDict = new Dictionary<int, int>();
        }
        public override void Clear()
        {
          
        }
    }

}
