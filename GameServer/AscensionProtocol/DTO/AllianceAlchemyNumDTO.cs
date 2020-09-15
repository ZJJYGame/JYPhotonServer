using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AllianceAlchemyNumDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual Dictionary<int,int> AlchemyNum { get; set; }

        public override void Clear()
        {
            RoleID = 0;
            AlchemyNum = null;
        }
    }
}
