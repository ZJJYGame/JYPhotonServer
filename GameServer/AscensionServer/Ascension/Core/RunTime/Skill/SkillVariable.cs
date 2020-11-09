using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// canCast是否可释放技能：
    /// 检测规则：是否足够mana、hp等值，是否处于cd中；
    /// </summary>
    public class SkillVariable: Variable<C2SSkillInput>
    {
        /// <summary>
        /// 是否可释放；
        /// </summary>
        public bool CanCast { get; private set; }
        /// <summary>
        /// 是否处于CD状态
        /// </summary>
        public bool OnCoolTime { get; private set; }
        /// <summary>
        /// 技能Id；
        /// </summary>
        public int SkillId { get { return Value.SkillId; } }

    }
}
