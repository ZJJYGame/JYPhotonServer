using MessagePack;
using System;
namespace Protocol
{
    /// <summary>
    /// 实体容器结构体；
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public struct FixContainer 
    {
        [Key(0)]
        public byte ContainerType { get; set; }
        [Key(1)]
        public int ContainerId { get; set; }
    }
}