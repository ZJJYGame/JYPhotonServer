using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class S2CPlayerSkill : IDataContract
{
        /// <summary>
        /// 实体容器ID，例如房间实体号，场景实体号等等；
        /// </summary>
        [Key(0)]
        public FixContainer Container { get; set; }
        [Key(1)]
        public List<C2SSkillInput> PlayerSkillList { get; set; }
    }
}
