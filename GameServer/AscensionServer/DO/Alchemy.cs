using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Alchemy : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual string AlchemyStatusDict { get; set; }

        public override void Clear()
        {
            RoleID = -1;
            AlchemyStatusDict=null;
        }
    }

}
