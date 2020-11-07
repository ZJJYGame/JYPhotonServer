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
    public class C2SSkillInput : C2SInput
    {
        /// <summary>
        /// 技能的Id；
        /// </summary>
        [Key(4)]
        public int SkillId{ get; set; }
        /// <summary>
        /// 技能种类；
        /// 例如：主动、被动、持续释放、开关类型；
        /// </summary>
        [Key(5)]
        public byte SkillType;
        /// <summary>
        /// 技能CD；
        /// </summary>
        [Key(6)]
        public int SkillCD;
        /// <summary>
        /// 技能持续时间；
        /// </summary>
        [Key(7)]
        public int SkillDuration;
        /// <summary>
        /// 技能使用后消耗；
        /// </summary>
        [Key(8)]
        public List<FixAffectValue> Costs { get; set; }
        /// <summary>
        /// 技能使用后增益
        /// </summary>
        [Key(9)]
        public List<FixAffectValue> Bouns{ get; set; }
    }
}