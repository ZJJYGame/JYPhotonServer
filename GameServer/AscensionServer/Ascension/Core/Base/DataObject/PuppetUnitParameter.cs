using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    [Serializable]
    [ConfigData]
    public class PuppetUnitParameter
    {
        public int PuppetID { get; set; }
        public List<int> PuppetAttributeMin { get; set; }
        public List<int> PuppetAttributeMax { get; set; }
        public int PuppetDurable { get; set; }
        /// <summary>
        /// 随机技能概率
        /// </summary>
        public int PuppetSkillProbability { get; set; }
        public List<int> PuppetSkillpoor { get; set; }
        public int PuppetTypeDetail { get; set; }
        /// <summary>
        /// 固定技能
        /// </summary>
        public int FixedSkill { get; set; }
        public int AdditionalAttributeProbability { get; set; }
        public int AttributeType { get; set; }
        public List<int> AdditionalMaxPercentage { get; set; }
    }
}
