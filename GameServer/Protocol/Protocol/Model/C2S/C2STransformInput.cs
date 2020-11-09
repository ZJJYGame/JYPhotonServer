using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    [MessagePackObject]
    public class C2STransformInput:C2SInput
    {
        [Key(4)]
        public int Tick { get; set; }
        [Key(5)]
        public long TS { get; set; }
        public C2STransformInput DeepClone()
        {
            return new C2STransformInput()
            {
                SessionId = this.SessionId,
                PlayerId = this.PlayerId,
                EntityContainer = this.EntityContainer,
                Tick = this.Tick,
                TS = this.TS
            };
        }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; RoomId:{EntityContainer} ; Tick : {Tick}";
        }
    }
}
