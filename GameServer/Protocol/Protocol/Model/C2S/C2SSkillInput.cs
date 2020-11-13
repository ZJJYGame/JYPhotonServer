using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Protocol
{
    [Serializable]
    /// <summary>
    /// 输入协议；
    /// </summary>
    [MessagePackObject]
    public class C2SSkillInput : IDataContract
    {
        /// <summary>
        /// 技能的Id；
        /// </summary>
        [Key(0)]
        public int SkillId { get; set; }
    }
}