using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CInput : IDataContract
    {
        [Key(0)]
        public long Tick { get; set; }
        /// <summary>
        /// 实体容器ID，例如房间实体号，场景实体号等等；
        /// </summary>
        [Key(1)]
        public int ContainerId { get; set; }
        [Key(2)]
        public Dictionary<int, CmdInput> InputDict { get; set; }
        [Key(3)]
        public long TS { get; set; }
        public void Clear()
        {
            Tick = 0;
            ContainerId = 0;
            InputDict = null;
            TS = 0;
        }
    }
}
