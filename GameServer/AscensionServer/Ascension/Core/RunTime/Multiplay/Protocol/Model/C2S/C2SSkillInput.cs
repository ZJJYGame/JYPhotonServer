using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
namespace AscensionServer
{
    [Serializable]
    /// <summary>
    /// 输入协议；
    /// </summary>
    [MessagePackObject(true)]
    public class C2SSkillInput : IDataContract
    {
        /// <summary>
        /// 技能的Id；
        /// </summary>
        public int SkillId { get; set; }
    }
}