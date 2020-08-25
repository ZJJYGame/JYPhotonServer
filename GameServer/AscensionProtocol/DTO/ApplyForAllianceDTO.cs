using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 用于传输仙盟申请列表的数据类型
    /// </summary>
    [Serializable]
    public class ApplyForAllianceDTO : DataTransferObject
    {
        public virtual string MemberName { get; set; }
        public virtual int Level  { get; set; }
        public virtual int School { get; set; }
        public virtual int RoleID { get; set; }

        public override void Clear()
        {
            MemberName = null;
            Level = 0;
            School = 0;
            RoleID = 0;
        }
    }
}
