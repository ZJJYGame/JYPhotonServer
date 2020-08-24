using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AllianceMemberDTO : DataTransferObject
    {
        public virtual int AllianceID { get; set; }
        public virtual List<int> ApplyforMember { get; set; }
        public virtual List<int> Member { get; set; }


        public override void Clear()
        {
            AllianceID = -1;
            ApplyforMember = null;
            Member = null;
        }
    }
}
