using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CChatMessage:IDataContract
    {
        [Key(0)]
        public int SenderSessionId { get; set; }
        [Key(1)]
        public int SenderPlayerId { get; set; }
        [Key(2)]
        public int ReceiverPlayerId { get; set; }
        [Key(3)]
        public int ReceiverSessionId { get; set; }
        [Key(4)]
        public object Message { get; set; }
    }
}
