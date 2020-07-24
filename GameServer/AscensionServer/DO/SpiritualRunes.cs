using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
   public  class SpiritualRunes : DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual int JobLevel { get; set; }
        public virtual string Recipe_Array { get; set; }
        public SpiritualRunes()
        {
            JobLevel = 5;
            Recipe_Array = "15001";
        }

        public override void Clear()
        {
            RoleID = -1;
            JobLevel = 0;
            Recipe_Array = null;
        }
    }
}
