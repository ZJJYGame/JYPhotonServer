using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleAuctionItemsDTO : DataTransferObject
    {
        public int RoleID { get; set; }
        public List<int> AuctionItemsGuidList { get; set; }
        public override void Clear()
        {
            RoleID = 0;
            AuctionItemsGuidList = null;
        }
    }
}
