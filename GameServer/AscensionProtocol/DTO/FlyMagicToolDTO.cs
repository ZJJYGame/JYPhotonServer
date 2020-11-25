using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class FlyMagicToolDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual List<int>  AllFlyMagicTool{ get; set; }
        public virtual FlyMagicToolType OprateType { get; set; }
        public virtual int FlyMagicToolID { get; set; }

        public override void Clear()
        {
            RoleID = -1;
            AllFlyMagicTool.Clear();
            OprateType = FlyMagicToolType.Noen;
        }

        public enum FlyMagicToolType
        {
            Noen=0,
            Add=1,
            Get=2,
        }
    }
}
