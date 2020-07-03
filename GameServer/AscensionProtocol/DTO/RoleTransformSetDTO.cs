using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    /// <summary>
    /// RoleTransformDTO的信息队列
    /// </summary>
    [Serializable]
    public class RoleTransformSetDTO : DataTransferObject
    {
        public RoleTransformSetDTO()
        {
            RoleTransformSet = new Queue<RoleTransformDTO>();
        }
        public int RoleID { get; set; }
        public Queue<RoleTransformDTO> RoleTransformSet { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            RoleTransformSet.Clear();
        }
    }
}
