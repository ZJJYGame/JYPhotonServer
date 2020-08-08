using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{

    public class ShoppingMallDTO : DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual int Discount { get; set; }//折扣
        public virtual int SelfLimitQuantity { get; set; }//个人限量
        public virtual int Price { get; set; }//价格
        public virtual int GoodsID { get; set; }//物品id
        public override void Clear()
        {
            RoleID = -1;
            Discount = 0;
            SelfLimitQuantity = 0;
            Price = 0;
            GoodsID = 0;
            
        }
    }
}
