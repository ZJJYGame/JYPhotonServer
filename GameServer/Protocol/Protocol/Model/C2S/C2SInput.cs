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
        public FixContainer EntityContainer { get; set; }
        /// <summary>
        /// 玩家角色的输入；
        /// key:输入的类型,value:输入的数据；
        /// </summary>
        [Key(3)]
        public Dictionary<string, IDataContract> InputDataDict { get; set; }
        public override string ToString()
        {
            return $"SessionId:{SessionId} ; RoomId:{EntityContainer} ;";
        }
    }
}
