using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RingDTO: ProtocolDTO
    {
        public virtual int ID { set; get; }
        public virtual int RingId { get; set; }
        public virtual HashSet< RingItemsDTO> RingItems { get; set; }
    }
    [Serializable]
    public class RingItemsDTO
    {
        public virtual int RingItemId { get; set; }
        public virtual int RingItemCount { get; set; }
        public virtual string RingItemAdorn { get; set; }
    }
}
