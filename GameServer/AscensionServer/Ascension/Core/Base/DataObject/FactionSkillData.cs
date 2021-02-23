using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //宗门藏经阁兑换
    [Serializable]
    [ConfigData]
    public class FactionSkillData
    {
        public int SchoolID { get; set; }
        public List<FactionSkill> FactionSkill { get; set; }
    }
    [Serializable]
    [ConfigData]
    public class FactionSkill
    {
        public int FactionItem_ID { get; set; }
        public string FactionItem_Name { get; set; }
        public int School_ID { get; set; }
        public string FactionItem_ImgName { get; set; }
        public FactionSkillType FactionItem_Type { get; set; }
        public int FactionItem_LevelID { get; set; }
        public int FactionItem_TodayNum { get; set; }
        public int FactionItem_Number { get; set; }
        public bool Is_Can_Exchange { get; set; }
        public string FactionItem_LevelName { get; set; }
        public Faction_LevelType FactionItem_Level { get; set; }
        public int FactionItem_TodayIndex = 0;
    }

    public enum FactionSkillType
    {
        GongFa,
        MiShu
    }
}


