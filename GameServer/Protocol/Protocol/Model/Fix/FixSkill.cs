//***********************************************************
// 描述：
// 作者：Don  
// 创建时间：2020-10-26 16:13:53
// 版 本：1.0
//***********************************************************
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Protocol
{
    [MessagePackObject]
    [Serializable]
    public class FixSkill
    {
        /// <summary>
        /// 全局技能Id
        /// </summary>
        [Key(0)]
        public int SkillId { get; set; }
    }
}