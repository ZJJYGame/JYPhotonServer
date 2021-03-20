using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AscensionProtocol.DTO
{
    /// <summary>
    /// 宗门丹药数据
    /// </summary>
    [Serializable]
    public class AllianceAlchemyNumDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        /// <summary>
        ///兑换过的丹药数据
        /// </summary>
        public virtual Dictionary<int,int> AlchemyNum { get; set; }

        public override void Clear()
        {
            RoleID = 0;
            AlchemyNum = null;
        }
    }
}
