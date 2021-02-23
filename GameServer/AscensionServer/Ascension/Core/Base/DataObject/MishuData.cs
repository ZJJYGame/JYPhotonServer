using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 秘术的ID
    /// 秘术基础数值列表
    /// </summary>
    [Serializable]
    [ConfigData]
    public class MiShuData
    {
        public int Mishu_ID { get; set; }
        public List<MishuSkillData> mishuSkillDatas { get; set; }
    }
    /// <summary>
    /// 秘术详细数值类
    /// yzq添加
    /// </summary>
    [Serializable]
    [ConfigData]
    public class MishuSkillData 
    {
        public int Mishu_Floor { get; set; }
        public int Need_Level_ID { get; set; }
        public List<int> Skill_Array_One { get; set; }
        public List<int> Skill_Array_Two { get; set; }
        public int Exp_Level_Up { get; set; }
        public int Role_HP { get; set; }
        public int Role_MP { get; set; }
        public int Role_Soul { get; set; }
        public int Best_Blood { get; set; }
        public int Attact_Speed { get; set; }
        public int Attact_Physical { get; set; }
        public int Defend_Physical { get; set; }
        public int Attact_Power { get; set; }
        public int Defend_Power { get; set; }
        public float Attact_Soul { get; set; }
        public float Defend_Soul { get; set; }
        public int Up_Double { get; set; }
        public int Down_Double { get; set; }
        public int Move_Speed { get; set; }
        public int Role_Popularity { get; set; }
        public int Value_Hide { get; set; }
    }
}


