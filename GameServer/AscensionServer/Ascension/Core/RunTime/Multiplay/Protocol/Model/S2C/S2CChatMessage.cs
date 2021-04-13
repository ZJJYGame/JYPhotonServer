using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Protocol
{
    [Serializable]
    [MessagePackObject(true)]
    public class S2CChatMessage:IDataContract
    {
        public int SenderSessionId { get; set; }
        public int SenderPlayerId { get; set; }
        public int ReceiverPlayerId { get; set; }
        public int ReceiverSessionId { get; set; }
        public object Message { get; set; }
    }
}
