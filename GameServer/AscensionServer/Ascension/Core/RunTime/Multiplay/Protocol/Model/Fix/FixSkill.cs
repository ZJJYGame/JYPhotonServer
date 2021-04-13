using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
namespace AscensionServer
{
    [MessagePackObject(true)]
    [Serializable]
    public class FixSkill
    {
        /// <summary>
        /// 全局技能Id
        /// </summary>
        public int SkillId { get; set; }
    }
}