using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class AllianceExchangeGoodsDTO: DataTransferObject
    {
        public virtual int AllianceID { get; set; }
        public virtual Dictionary<int, int> ExchangeGoods { get; set; }

        public override void Clear()
        {
            AllianceID = -1;
            ExchangeGoods = new Dictionary<int, int>();
        }
    }
}
