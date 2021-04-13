using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
namespace AscensionServer
{
    [Serializable]
    /// <summary>
    /// 输入协议；
    /// </summary>
    [MessagePackObject(true)]
    public class CmdInput 
    {
        public int RoleId { get; set; }
        /// <summary>
        /// 实体容器，例如房间实体，场景实体等等；
        /// </summary>
        public FixContainer EntityContainer { get; set; }
        public override string ToString()
        {
            return $"RoleId{RoleId}";
        }
    }
}
