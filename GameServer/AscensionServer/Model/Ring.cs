/*
*Author   Don
*Since 	2020-04-20
*Description 玩家当前储物戒指映射模型
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Model
{
    [Serializable]
    public class Ring
    {
        public virtual int ID { set; get; }
        public virtual int RingId { get; set; }
        public virtual int RingItemId { get; set; }
        public virtual int RingItemCount { get; set; }
        public virtual bool RingItemAdorn { get; set; }
    }
}

