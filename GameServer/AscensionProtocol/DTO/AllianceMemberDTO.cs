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
        /// <summary>
        /// 宗门申请人员列表
        /// </summary>
        public virtual List<int> ApplyforMember { get; set; }
        /// <summary>
        /// 宗门成员列表
        /// </summary>
        public virtual List<int> Member { get; set; }


        public override void Clear()
        {
            AllianceID = -1;
            ApplyforMember = null;
            Member = null;
        }
    }
}
