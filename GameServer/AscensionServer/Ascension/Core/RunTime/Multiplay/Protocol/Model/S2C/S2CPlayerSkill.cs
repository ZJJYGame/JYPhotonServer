using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public class S2CPlayerSkill 
{
        /// <summary>
        /// 实体容器ID，例如房间实体号，场景实体号等等；
        /// </summary>
        public FixContainer Container { get; set; }
        public List<C2SSkillInput> PlayerSkillList { get; set; }
    }
}
