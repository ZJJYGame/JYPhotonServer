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
        public int RoleID { get; set; }
        public Queue<TransformDTO> TransformSet { get; set; }
        public RoleTransformSetDTO()
        {
            RoleID = -1;
            TransformSet = new Queue<TransformDTO>();
        }
        public override void Clear()
        {
            RoleID = -1;
            TransformSet.Clear();
        }
    }
}
