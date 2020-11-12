using MessagePack;
using System;
namespace Protocol
{
    /// <summary>
    /// 实体容器结构体；
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public struct FixEntityContainer 
    {
        [Key(0)]
        public byte EntityContainerType { get; set; }
        [Key(1)]
        public int EntityContainerId { get; set; }
    }
}