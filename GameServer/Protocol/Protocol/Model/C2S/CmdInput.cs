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
    public class CmdInput : IDataContract
    {
        [Key(0)]
        public int RoleId { get; set; }
        /// <summary>
        /// 实体容器，例如房间实体，场景实体等等；
        /// </summary>
        [Key(1)]
        public FixContainer EntityContainer { get; set; }
        /// <summary>
        /// 玩家角色的输入；
        /// key:输入的类型,value:输入的数据；
        /// </summary>
        [Key(2)]
        public Dictionary<byte, ICmdDataContract> CmdDict { get; set; }
        public override string ToString()
        {
            return $"RoleId{RoleId}";
        }
    }
}
