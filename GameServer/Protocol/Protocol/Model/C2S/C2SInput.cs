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
        /// 实体容器，例如房间实体，场景实体等等；
        /// </summary>
        [Key(2)]
        public FixEntityContainer EntityContainer { get; set; }
        [Key(3)]
        public List<IDataContract> InputStream { get; set; }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; RoomId:{EntityContainer} ;";
        }
    }
}
