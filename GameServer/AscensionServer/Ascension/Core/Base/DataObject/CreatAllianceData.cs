using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 创建宗门所需条件，及创建初始化数据
    /// </summary>
    [Serializable]
    [ConfigData]
    public class CreatAllianceData
    {
        public int ID { get; set; }
        public int RoleLevel { get; set; }
        public int SpiritStones { get; set; }
        public int AllianceLevel { get; set; }
        public int Chamber { get; set; }
        public int ArmsDrillSite { get; set; }
        public int ScripturesPlatform { get; set; }
        public int AlchemyStorage { get; set; }
        public int Popularity { get; set; }
        public int AlliancePeople { get; set; }
    }
}
