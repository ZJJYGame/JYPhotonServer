﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RingDTO: DataTransferObject
    {
        public virtual int ID { set; get; }
        public virtual int RingId { get; set; }
        public virtual Dictionary<int,RingItemsDTO> RingItems { get; set; }
        public override void Clear()
        {
            ID = -1;
            RingId = 0;
            RingItems.Clear();
        }
    }
    [Serializable]
    public class RingItemsDTO
    {
        public virtual int RingItemCount { get; set; }
        public virtual string RingItemAdorn { get; set; }
    }
}
