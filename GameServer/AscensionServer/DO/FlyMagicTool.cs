using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class FlyMagicTool: DataObject
    {
        public virtual int RoleID { get; set; }
        public virtual string  AllFlyMagicTool { get; set; }

        public FlyMagicTool()
        {
            AllFlyMagicTool = null;
        }

        public override void Clear()
        {
            RoleID = -1;
            AllFlyMagicTool=null;
        }
    }
}
