using MessagePack;
using System;
namespace AscensionServer
{
    /// <summary>
    /// 实体容器结构体；
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public struct FixContainer 
    {
        public byte ContainerType { get; set; }
        public int ContainerId { get; set; }
    }
}