using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    public class SecondaryJobDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int UseItemID { get; set; }
        public virtual AlchemyDTO AlchemyDTO { get; set; }
        public virtual ForgeDTO ForgeDTO { get; set; }
        public virtual SecondaryJobType JobType { get; set; }

        public override void Clear()
        {
            UseItemID = 0;
            AlchemyDTO.Clear();
            ForgeDTO.Clear();
        }
        public enum SecondaryJobType
        {
            None=0,
            Ahclemy = 1,
            Forge = 2

        }

        public enum JobOperateType
        {
            None = 0,
            Get=1,
            Update=2,
            Compound=3,
            Level=4
        }
    }
}
