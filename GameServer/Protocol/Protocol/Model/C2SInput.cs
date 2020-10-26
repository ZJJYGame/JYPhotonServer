using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
namespace Protocol
{
    [Serializable]
    /// <summary>
    /// 输入协议；
    /// </summary>
    [MessagePackObject]
    public class C2SInput : IDataContract
    {
        [Key(0)]
        public int SessionId { get; set; }
        [Key(1)]
        public int PlayerId { get; set; }
        /// <summary>
        /// 实体容器ID，例如房间实体号，场景实体号等等；
        /// </summary>
        [Key(2)]
        public int EntityContainerId { get; set; }
        [Key(3)]
        public int Tick { get; set; }
        [Key(4)]
        public List<IDataContract> InputStream { get; set; }
        [Key(5)]
        public long TS { get; set; }
        public C2SInput DeepClone()
        {
            return new C2SInput()
            {
                SessionId = this.SessionId,
                PlayerId = this.PlayerId,
                EntityContainerId = this.EntityContainerId,
                Tick = this.Tick,
                TS = this.TS
            };
        }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; RoomId:{EntityContainerId} ; Tick : {Tick}";
        }
    }
}
